using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using OfficeOpenXml;

public class DataTablesExporter
{
    private string tablesPath;
    string[][] mDataExportSettings;
    readonly Dictionary<string, List<int>> mDataExcelSheetMap = new Dictionary<string, List<int>>();
    List<string> mOutPath = new();
    List<string> mOutFileNames = new();


    private struct DataTableParams
    {
        public List<string> TablesHeaders;
        public List<string> TablesTypes;
        public List<string> TableHeadComments;
        public string[,] TablesData;
        public int ColNum;
        public int RowNum;
        public DataTableParams(List<string> tablesHeaders, List<string> tablesTypes, List<string> tableHeadComments, string[,] tablesData, int colNum, int rowNum)
        {
            TablesHeaders = tablesHeaders;
            TablesTypes = tablesTypes;
            TableHeadComments = tableHeadComments;
            TablesData = tablesData;
            ColNum = colNum;
            RowNum = rowNum;
        }
    }

    public DataTablesExporter(string configTableName, string tableSheetName, string tablesPath)
    {
        this.tablesPath = tablesPath;
        InitDataTableSettings(configTableName, tableSheetName);
        ClearDataTableCreatedFiles();
    }

    private void InitDataTableSettings(string configTableName, string tableSheetName)
    {
        ExcelPackage configTables = ExportTableUtil.ReadExcel(tablesPath + "/" + Path.GetFileNameWithoutExtension(configTableName));
        if (configTables == null)
        {
            throw new Exception(tablesPath + "/" + Path.GetFileNameWithoutExtension(configTableName) + "路径为空!");
        }
        ExcelWorksheet sheet = configTables.Workbook.Worksheets[tableSheetName];
        mDataExcelSheetMap.Clear();
        mOutPath.Clear();

        int RowNum = sheet.Dimension.Rows - 1;
        mDataExportSettings = new string[RowNum][];
        for (int i = 0; i < RowNum; i++)
        {
            mDataExportSettings[i] = new string[10];
            for (int j = 0; j < sheet.Dimension.Columns; j++)
            {
                mDataExportSettings[i][j] = ExportTableUtil.GetCell(sheet, i + 1, j);
            }
            if (!mDataExcelSheetMap.ContainsKey(mDataExportSettings[i][0]))
            {
                mDataExcelSheetMap.Add(mDataExportSettings[i][0], new List<int>());
            }

            if (!mOutPath.Contains(mDataExportSettings[i][3]))
            {
                mOutPath.Add(mDataExportSettings[i][3]);
            }
            if (!mOutFileNames.Contains(mDataExportSettings[i][2]))
            {
                mOutFileNames.Add($"{mDataExportSettings[i][3]}{mDataExportSettings[i][2]}.cs");
            }

            mDataExcelSheetMap[mDataExportSettings[i][0]].Add(i);
        }
        ExportTableUtil.MaxTablesCount(mDataExcelSheetMap.Count);
    }

    private void ClearDataTableCreatedFiles()
    {
        foreach (var outPath in mOutPath)
        {
            foreach (var fileName in Directory.GetFiles(outPath))
            {
                if (!mOutFileNames.Contains(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }
    }

    public void ExportDataTable()
    {
        foreach (KeyValuePair<string, List<int>> p in mDataExcelSheetMap)
        {
            LocalTablesExporter.RegisterExportTask(() =>
            {
                DataTableTask(p);
            }, p.Key);
        }
    }

    private void DataTableTask(KeyValuePair<string, List<int>> input)
    {
        List<int> sheetList = input.Value;
        foreach (int sheetIndex in sheetList)
        {
            string[] exportSetting = mDataExportSettings[sheetIndex];
            if (string.IsNullOrEmpty(exportSetting[0]) || string.IsNullOrEmpty(exportSetting[1]) || string.IsNullOrEmpty(exportSetting[2]))
            {
                throw new Exception("TablesConfig配置错误 行数 = " + sheetIndex);
            }
            string excel_name = exportSetting[0];
            string sheet_name = exportSetting[1];
            string out_client_code_path = $"{exportSetting[3]}{exportSetting[2]}.cs";
            string out_server_code_path = $"{ExportTableUtil.SERVER_CS_DIR}/{exportSetting[2]}.cs";
            string data_name = exportSetting[4];
            string packet_name = exportSetting[5];

            bool export_server = string.IsNullOrEmpty(exportSetting[6]) ? false : (int.TryParse(exportSetting[6], out var val) && val == 1);
            DataTableParams tableParams = new DataTableParams(new List<string>(), new List<string>(), new List<string>(), default, 0, 0);
            ParseTablesData(excel_name, sheet_name, ref tableParams);
            PacketEditor.Create(packet_name);
            PacketEditor.AddFile(packet_name, data_name, ExportDataToBytes(excel_name, sheet_name, tableParams));
            var data = ExportDataCSParse(excel_name, sheet_name, Path.GetFileNameWithoutExtension(out_client_code_path), Path.GetFileName(data_name), packet_name, tableParams);
            var outServerPath = export_server ? out_server_code_path : string.Empty;
            if (File.Exists(out_client_code_path))
            {
                File.Delete(out_client_code_path);
            }
            File.WriteAllBytes(out_client_code_path, data);
            if (!string.IsNullOrEmpty(outServerPath))
            {
                File.Copy(out_client_code_path, outServerPath, true);
            }
        }
    }

    private void ParseTablesData(string excel_name, string sheet_name, ref DataTableParams tableParams)
    {
        string file_path = tablesPath + "/" + excel_name;
        ExcelPackage ConfigTables = ExportTableUtil.ReadExcel(file_path);
        ExcelWorksheet sheet = ConfigTables.Workbook.Worksheets[sheet_name];
        if (sheet == null)
        {
            throw new Exception($"{file_path}中找不到表:{sheet_name}");
        }
        if (sheet.Dimension.Rows < 3)
        {
            throw new Exception("文件格式不对:" + file_path + " Sheet:" + sheet_name);
        }

        List<string> TableMainKeys = new List<string>();

        //head name
        for (int col_num = 0; col_num < sheet.Dimension.Columns; col_num++)
        {
            string str = ExportTableUtil.GetCell(sheet, 0, col_num);
            if (string.IsNullOrEmpty(str))
            {
                break;
            }
            if (!ExportTableUtil.CheckHead(str, tableParams.TablesHeaders))
            {
                throw new Exception($"转换表:{excel_name}[{sheet_name}] 第{0}行{col_num}列失败! \n 表项名字是 非法（字母开头只能用字母数字下划线）或 重复 或 保留关键字");
            }
            tableParams.TablesHeaders.Add(str);
            tableParams.ColNum++;
        }
        tableParams.TablesData = new string[sheet.Dimension.Rows - 3, tableParams.ColNum];

        for (int row_num = 1; row_num < sheet.Dimension.Rows; row_num++)
        {
            if (row_num > 2)
            {
                if (string.IsNullOrEmpty(ExportTableUtil.GetCell(sheet, row_num, 0)))   //如果首行是空的话。。则表示表已经结束
                {
                    break;
                }
                tableParams.RowNum++;
            }
            for (int col_num = 0; col_num < tableParams.ColNum; col_num++)
            {
                string str = ExportTableUtil.GetCell(sheet, row_num, col_num);

                if (row_num == 1) //type
                {
                    tableParams.TablesTypes.Add(str);
                }
                else if (row_num == 2)//Comments
                {
                    tableParams.TableHeadComments.Add(str);
                }
                else //data
                {
                    if (col_num == 0)
                    {
                        if (TableMainKeys.Contains(str))
                        {
                            throw new Exception($"转换表:{excel_name}[{sheet_name}] 第{row_num}行{col_num}列失败! \n ID:{tableParams.TablesHeaders[0]} {str} 重复！");
                        }
                        TableMainKeys.Add(str);
                    }
                    tableParams.TablesData[row_num - 3, col_num] = str;
                }
            }
        }
    }


    private byte[] ExportDataCSParse(string excel_name, string sheet_name, string class_name, string data_file_name, string data_packet_name, DataTableParams tableParams)
    {
        var fs = new MemoryStream();
        StreamWriter bw = new StreamWriter(fs, Encoding.UTF8);

        bw.Write("using System;" + "\n");
        bw.Write("using System.Collections.Generic;" + "\n");
        bw.Write("\n");
        bw.Write("public partial class " + class_name + " : TableData" + "\n");
        bw.Write("{" + "\n");
        bw.Write("\t" + "public readonly string sFilePath = " + "\"" + data_file_name + "\";" + "\n");
        bw.Write("\t" + "public Dictionary<" + tableParams.TablesTypes[0] + ", tData> mData;" + "\n");
        bw.Write("\t" + "public List<tData> DataList;" + "\n");
        bw.Write("\t" + "public partial struct tData" + "\n");
        bw.Write("\t" + "{" + "\n");
        for (int i = 0; i < tableParams.ColNum; i++)
        {
            if (tableParams.TablesHeaders[i].StartsWith("#") || tableParams.TablesTypes[i].StartsWith("#"))
            {
                continue;
            }
            if (tableParams.TableHeadComments[i] != null && tableParams.TableHeadComments[i] != "")
            {
                bw.Write("\t\t" + "/// <summary>" + "\n");

                if (tableParams.TableHeadComments[i].Contains("\r\n"))
                {
                    string[] arr = tableParams.TableHeadComments[i].Split("\r\n");

                    foreach (string str in arr)
                    {
                        bw.Write("\t\t" + "///" + str + "\n");
                    }
                }
                else if (tableParams.TableHeadComments[i].Contains("\r"))
                {
                    string[] arr = tableParams.TableHeadComments[i].Split("\r");

                    foreach (string str in arr)
                    {
                        bw.Write("\t\t" + "///" + str + "\n");
                    }
                }
                else if (tableParams.TableHeadComments[i].Contains("\n"))
                {
                    string[] arr = tableParams.TableHeadComments[i].Split("\n");

                    foreach (string str in arr)
                    {
                        bw.Write("\t\t" + "///" + str + "\n");
                    }
                }
                else
                {
                    bw.Write("\t\t" + "///" + tableParams.TableHeadComments[i] + "\n");
                }

                bw.Write("\t\t" + "/// </summary>" + "\n");
            }

            if (tableParams.TablesTypes[i] == "enum")
            {
                var enum_type = $"E{tableParams.TablesHeaders[i]}";
                bw.Write("\t\t" + "public " + enum_type + " " + tableParams.TablesHeaders[i] + ";\n");
            }
            else if (tableParams.TablesTypes[i].StartsWith("enum:"))
            {
                var enumType = tableParams.TablesTypes[i].Split(':');
                bw.Write("\t\t" + "public " + enumType[1] + " " + tableParams.TablesHeaders[i] + ";\n");
            }
            else
            {
                bw.Write("\t\t" + "public " + tableParams.TablesTypes[i] + " " + tableParams.TablesHeaders[i] + ";\n");
            }
        }

        bw.Write("\t\tpublic bool IsNull\n");
        bw.Write("\t\t{\n");
        bw.Write("\t\t\tget \n");
        bw.Write("\t\t\t{\n");
        if (tableParams.TablesTypes[0].ToLower() != "string")
        {
            bw.Write($"\t\t\t\treturn {tableParams.TablesHeaders[0]} <= 0;\n");
        }
        else
        {
            bw.Write($"\t\t\t\treturn string.IsNullOrEmpty({tableParams.TablesHeaders[0]});\n");
        }
        bw.Write("\t\t\t}\n");
        bw.Write("\t\t}\n");

        bw.Write("\t" + "}" + "\n\n");
        bw.Write($"\tpublic {class_name}()\n");
        bw.Write("\t{\n");
        bw.Write("\t\ttry\n");
        bw.Write("\t\t{\n");
        bw.Write("\t\t\tPACKET_NAME = \"" + data_packet_name + "\";\n");
        bw.Write("\t\t\tReadTable();\n");
        bw.Write("\t\t\tParseData();\n");
        bw.Write("\t\t}\n");
        bw.Write("\t\tcatch(Exception exp)\n");
        bw.Write("\t\t{\n");
        bw.Write("\t\t\tG.Log($\"LOAD TABLE ERROR!{exp}\");\n");
        bw.Write("\t\t}\n");
        bw.Write("\t}\n\n");

        bw.Write("\t" + "void ReadTable()" + "\n");
        bw.Write("\t" + "{" + "\n");
        bw.Write("\t\t" + "if (mTableData == null)" + "\n");
        bw.Write("\t\t" + "{" + "\n");
        bw.Write("\t\t\t" + "mTableData = new RawTable();" + "\n");
        bw.Write("\t\t" + "}" + "\n");
        bw.Write("\t\t" + $"mTableData.readBinary(sFilePath, PACKET_NAME);" + "\n");
        bw.Write("\t" + "}" + "\n");
        bw.Write("\t" + "void ParseData()" + "\n");
        bw.Write("\t" + "{" + "\n");
        bw.Write("\t\t" + "mData = new Dictionary<" + tableParams.TablesTypes[0] + ", tData>(mTableData._nRows);" + "\n");
        bw.Write("\t\t" + "DataList = new List<tData>(mTableData._nRows);" + "\n");
        bw.Write("\t\t" + "for (int i = 0; i < mTableData._nRows; i++)" + "\n");
        bw.Write("\t\t" + "{" + "\n");
        bw.Write("\t\t\t" + "tData data = new tData();" + "\n");

        int j = 0;
        for (int i = 0; i < tableParams.TablesHeaders.Count; i++)
        {
            if (tableParams.TablesHeaders[i].StartsWith("#") || tableParams.TablesTypes[i].StartsWith("#"))
            {
                continue;
            }
            string value_name = tableParams.TablesHeaders[i];
            string value_type = tableParams.TablesTypes[i];
            string str_func = string.Empty;
            if (value_type.StartsWith("enum:"))
            {
                var enum_type = value_type.Split(':')[1];
                str_func = $"({enum_type})mTableData.GetEnum(i, {j});";
            }
            else
            {
                switch (value_type)
                {
                    case "string":
                        str_func = "mTableData.GetString(i," + j + ");";
                        break;
                    case "bool":
                        str_func = "mTableData.GetBool(i," + j + ");";
                        break;
                    case "byte":
                        str_func = "mTableData.GetByte(i," + j + ");";
                        break;
                    case "sbyte":
                        str_func = "mTableData.GetSByte(i," + j + ");";
                        break;
                    case "short":
                        str_func = "mTableData.GetShort(i," + j + ");";
                        break;
                    case "ushort":
                        str_func = "mTableData.GetUShort(i," + j + ");";
                        break;
                    case "int":
                        str_func = "mTableData.GetInt(i," + j + ");";
                        break;
                    case "uint":
                        str_func = "mTableData.GetUInt(i," + j + ");";
                        break;
                    case "enum":
                        var enum_type = $"E{value_name}";
                        str_func = $"({enum_type})mTableData.GetEnum(i, {j});";
                        break;
                    case "long":
                        str_func = "mTableData.GetLong(i," + j + ");";
                        break;
                    case "ulong":
                        str_func = "mTableData.GetULong(i," + j + ");";
                        break;
                    case "float":
                        str_func = "mTableData.GetFloat(i," + j + ");";
                        break;
                    case "double":
                        str_func = "mTableData.GetDouble(i," + j + ");";
                        break;
                    case "DateTime":
                        str_func = "mTableData.GetDateTime(i," + j + ");";
                        break;
                    case "int[]":
                        str_func = "mTableData.GetIntArray(i," + j + ");";
                        break;
                    case "uint[]":
                        str_func = "mTableData.GetUIntArray(i," + j + ");";
                        break;
                    case "float[]":
                        str_func = "mTableData.GetFloatArray(i," + j + ");";
                        break;
                    case "double[]":
                        str_func = "mTableData.GetDoubleArray(i," + j + ");";
                        break;
                    case "string[]":
                        str_func = "mTableData.GetStringArray(i," + j + ");";
                        break;
                    case "int[][]":
                        str_func = "mTableData.GetIntArray2(i," + j + ");";
                        break;
                    case "uint[][]":
                        str_func = "mTableData.GetUIntArray2(i," + j + ");";
                        break;
                    case "float[][]":
                        str_func = "mTableData.GetFloatArray2(i," + j + ");";
                        break;
                    case "double[][]":
                        str_func = "mTableData.GetDoubleArray2(i," + j + ");";
                        break;
                    case "string[][]":
                        str_func = "mTableData.GetStringArray2(i," + j + ");";
                        break;
                    default:
                        throw new Exception($"转换表:{excel_name}[{sheet_name}]失败! \n不支持的类型=>属性名:{value_name},类型:{value_type}");
                }
            }
            bw.Write("\t\t\t" + "data." + value_name + " = " + str_func + "\n");
            j++;
        }


        bw.Write("\t\t\t" + "mData.Add(data." + tableParams.TablesHeaders[0] + ", data);" + "\n");
        bw.Write("\t\t\t" + "DataList.Add(data);" + "\n");
        bw.Write("\t\t" + "}" + "\n");
        bw.Write("\t\t" + " mTableData = null;" + "\n");
        bw.Write("\t" + "}" + "\n");
        bw.Write("}" + "\n");

        bw.Close();
        fs.Close();
        return fs.ToArray();
    }


    private byte[] ExportDataToBytes(string excel_name, string sheet_name, DataTableParams tableParams)
    {
        int ColNum = 0;
        for (int i = 0; i < tableParams.ColNum; i++)
        {
            if (tableParams.TablesHeaders[i].StartsWith("#") || tableParams.TablesTypes[i].StartsWith("#"))
            {
                continue;
            }
            ColNum++;
        }

        MemoryStream fs = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(fs, Encoding.UTF8);
        try
        {
            bw.Write(tableParams.RowNum);
            bw.Write(ColNum);

            for (int i = 0; i < tableParams.ColNum; i++)
            {
                if (tableParams.TablesHeaders[i].StartsWith("#") || tableParams.TablesTypes[i].StartsWith("#"))
                {
                    continue;
                }

                bw.Write(tableParams.TablesTypes[i]);
            }

            for (int col = 0; col < tableParams.ColNum; col++)
            {
                var value_type = tableParams.TablesTypes[col];
                var head = tableParams.TablesHeaders[col];
                if (head.StartsWith("#") || value_type.StartsWith("#"))
                {
                    continue;
                }
                if (value_type.StartsWith("enum:"))
                {
                    try
                    {
                        WriteEnum(value_type.Split(':')[1], col, bw, tableParams);
                    }
                    catch (Exception exp)
                    {
                        throw new Exception($"转换表:{excel_name}[{sheet_name}]失败!\n 属性名:{head},类型:{value_type} \n{exp}");
                    }
                }
                else if (value_type == "enum")
                {
                    try
                    {
                        WriteEnum($"E{tableParams.TablesHeaders[col]}", col, bw, tableParams);
                    }
                    catch (Exception exp)
                    {
                        throw new Exception($"转换表:{excel_name}[{sheet_name}]失败!\n 属性名:{head},类型:{value_type} \n{exp}");
                    }
                }
                else
                {
                    for (int row = 0; row < tableParams.RowNum; row++)
                    {
                        string value = tableParams.TablesData[row, col];
                        try
                        {
                            switch (value_type)
                            {
                                case "string":
                                    bw.Write(value);
                                    break;
                                case "bool":
                                    if (string.IsNullOrWhiteSpace(value))
                                    {
                                        bw.Write(false);
                                    }
                                    else
                                    {
                                        bw.Write(Convert.ToBoolean(value));
                                    }
                                    break;
                                case "byte":
                                    if (string.IsNullOrWhiteSpace(value))
                                    {
                                        bw.Write((byte)0);
                                    }
                                    else
                                    {
                                        bw.Write(Convert.ToByte(value));
                                    }
                                    break;
                                case "sbyte":
                                    if (string.IsNullOrWhiteSpace(value))
                                    {
                                        bw.Write((sbyte)0);
                                    }
                                    else
                                    {
                                        bw.Write(Convert.ToSByte(value));
                                    }
                                    break;
                                case "short":
                                    if (string.IsNullOrWhiteSpace(value))
                                    {
                                        bw.Write((short)0);
                                    }
                                    else
                                    {
                                        bw.Write(Convert.ToInt16(value));
                                    }
                                    break;
                                case "ushort":
                                    if (string.IsNullOrWhiteSpace(value))
                                    {
                                        bw.Write((ushort)0);
                                    }
                                    else
                                    {
                                        bw.Write(Convert.ToUInt16(value));
                                    }
                                    break;
                                case "int":
                                    if (string.IsNullOrWhiteSpace(value))
                                    {
                                        bw.Write((int)0);
                                    }
                                    else
                                    {
                                        bw.Write(Convert.ToInt32(value));
                                    }
                                    break;
                                case "uint":
                                    if (string.IsNullOrWhiteSpace(value))
                                    {
                                        bw.Write((uint)0);
                                    }
                                    else
                                    {
                                        bw.Write(Convert.ToUInt32(value));
                                    }
                                    break;
                                case "long":
                                    if (string.IsNullOrWhiteSpace(value))
                                    {
                                        bw.Write((long)0);
                                    }
                                    else
                                    {
                                        bw.Write(Convert.ToInt64(value));
                                    }
                                    break;
                                case "ulong":
                                    if (string.IsNullOrWhiteSpace(value))
                                    {
                                        bw.Write((ulong)0);
                                    }
                                    else
                                    {
                                        bw.Write(Convert.ToUInt64(value));
                                    }
                                    break;
                                case "float":
                                    if (string.IsNullOrWhiteSpace(value))
                                    {
                                        bw.Write((float)0);
                                    }
                                    else
                                    {
                                        bw.Write(Convert.ToSingle(value));
                                    }
                                    break;
                                case "double":
                                    if (string.IsNullOrWhiteSpace(value))
                                    {
                                        bw.Write((double)0);
                                    }
                                    else
                                    {
                                        bw.Write(Convert.ToDouble(value));
                                    }
                                    break;
                                case "DateTime":
                                    if (!DateTime.TryParseExact(value, "yyyy-MM-dd HH:mm:ss",
                                            CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                                    {
                                        throw new Exception("DateTime数据异常 " + tableParams.TablesHeaders[col] + " " + tableParams.TablesTypes[col]);
                                    }

                                    bw.Write(value);
                                    break;
                                case "int[]":
                                    ExportTableUtil.WriteIntArray(value, bw);
                                    break;
                                case "uint[]":
                                    ExportTableUtil.WriteUintArray(value, bw);
                                    break;
                                case "float[]":
                                    ExportTableUtil.WriteFloatArray(value, bw);
                                    break;
                                case "double[]":
                                    ExportTableUtil.WriteDoubleArray(value, bw);
                                    break;
                                case "string[]":
                                    ExportTableUtil.WriteStringArray(value, bw);
                                    break;
                                case "int[][]":
                                    ExportTableUtil.WriteIntArray2(value, bw);
                                    break;
                                case "uint[][]":
                                    ExportTableUtil.WriteUintArray2(value, bw);
                                    break;
                                case "float[][]":
                                    ExportTableUtil.WriteFloatArray2(value, bw);
                                    break;
                                case "double[][]":
                                    ExportTableUtil.WriteDoubleArray2(value, bw);
                                    break;
                                case "string[][]":
                                    ExportTableUtil.WriteStringArray2(value, bw);
                                    break;
                                default:
                                    throw new Exception("未识别的类型 = " + tableParams.TablesHeaders[col] + " " + tableParams.TablesTypes[col]);
                            }
                        }
                        catch (Exception exp)
                        {
                            throw new Exception($"转换表:{excel_name}[{sheet_name}]失败!\n第{row}行{col}列, 属性名:{head},类型:{value_type},值:{value}\n{exp}");
                        }
                    }
                }
            }
        }
        finally
        {
            bw.Close();
        }
        return fs.ToArray();
    }

    private void WriteEnum(string type, int i, BinaryWriter bw, DataTableParams tableParams)
    {
        if (ExportTableUtil.enumTypes.TryGetValue(type, out var enum_type))
        {
            for (int j = 0; j < tableParams.RowNum; j++)
            {
                var enum_value = tableParams.TablesData[j, i];
                if (string.IsNullOrEmpty(enum_value))
                {
                    var enum_values = Enum.GetValues(enum_type);
                    if (enum_values.Length > 0)
                    {
                        bw.Write((int)enum_values.GetValue(0));
                    }
                    else
                    {
                        throw new Exception($"不支持的枚举{enum_type}，参数{enum_value}。");
                    }
                }
                if (Enum.TryParse(enum_type, enum_value, true, out var value))
                {
                    bw.Write(value.GetHashCode());
                }
                else
                {
                    throw new Exception($"不支持的枚举{enum_type}，参数{enum_value}。");
                }
            }
        }
        else
        {
            throw new Exception("未识别的类型 = " + tableParams.TablesHeaders[i] + " " + tableParams.TablesTypes[i]);
        }
    }
}
