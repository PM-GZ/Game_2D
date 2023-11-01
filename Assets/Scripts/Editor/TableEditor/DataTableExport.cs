using System;
using System.IO;
using System.Collections.Generic;
using OfficeOpenXml;
using System.Text;
using System.Globalization;

public static class DataTableExport
{
    private struct TableData
    {
        public List<string> TableHeadList;
        public List<string> TypeList;
        public List<string> CommentList;
        public string[,] TableDatas;
        public int Row;
        public int Column;

        public TableData(List<string> tableHeadList, List<string> typeList, List<string> commentList, string[,] tableDatas, int row, int column)
        {
            TableHeadList = tableHeadList;
            TypeList = typeList;
            CommentList = commentList;
            TableDatas = tableDatas;
            Row = row;
            Column = column;
        }
    }


    private static string mTablePath;
    private static ExcelWorksheet mTable;
    private static string[,] mConfigTableDatas;
    private static Dictionary<string, string> mSheetPathDict;

    public static void ExportDataTable(string tablePath, ExcelPackage config)
    {
        mTablePath = tablePath;
        mTable = config.Workbook.Worksheets["Tables"];
        GetAllSheet();
        DeleteOldFiles();
        ExportData();
    }

    private static void GetAllSheet()
    {
        mConfigTableDatas = new string[mTable.Dimension.Rows, mTable.Dimension.Columns];
        mSheetPathDict = new();
        for (int i = 0; i < mTable.Dimension.Rows; i++)
        {
            for (int j = 0; j < mTable.Dimension.Columns; j++)
            {
                string value = ExportTableEditor.GetCellData(mTable, i + 1, j);

                if (j == 0 && string.IsNullOrEmpty(value))
                {
                    return;
                }

                mConfigTableDatas[i, j] = value;
            }
            string sheetPath = $"{mTablePath}{mConfigTableDatas[i, 0]}";
            mSheetPathDict.Add(sheetPath, mConfigTableDatas[i, 1]);
        }
    }

    private static void DeleteOldFiles()
    {
        for (int i = 0; i < mConfigTableDatas.GetLength(0); i++)
        {
            for (int j = 0; j < mConfigTableDatas.GetLength(1); j++)
            {
                string path = $"{mConfigTableDatas[i, 3]}{mConfigTableDatas[i, 2]}.cs";
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }

    private static void ExportData()
    {
        int index = 0;
        foreach (var dict in mSheetPathDict)
        {
            var tableData = GetTableData(dict.Key, dict.Value);
            string className = mConfigTableDatas[index, 2];
            string outPath = mConfigTableDatas[index, 3];
            string dataName = mConfigTableDatas[index, 4];
            string packetName = mConfigTableDatas[index, 5];
            var packetData = CreatePacket(dict.Key, dict.Value, tableData);
            var csData = CreateCSFile(dict.Key, dict.Value, className, dataName, packetName, tableData);
            File.WriteAllBytes($"{outPath}{className}.cs", csData);
            File.WriteAllBytes($"{ExportTableUtils.PacketPath}{packetName}", packetData);
            index++;
        }
    }

    private static TableData GetTableData(string tablePath, string sheetName)
    {
        var table = ExportTableEditor.GetConfigFile(tablePath);
        var sheet = table.Workbook.Worksheets[sheetName];
        if (sheet == null)
        {
            throw new Exception($"{tablePath}中找不到：{sheetName}");
        }
        if (sheet.Dimension.Rows < 3)
        {
            throw new Exception("文件格式不对:" + tablePath + " Sheet:" + sheetName);
        }

        List<string> keys = new List<string>();
        TableData data = new TableData(new(), new(), new(), default, 0, 0);
        data.TableDatas = new string[sheet.Dimension.Rows - 3, sheet.Dimension.Columns];
        data.Column = sheet.Dimension.Columns;
        for (int i = 0; i < sheet.Dimension.Rows; i++)
        {
            if (i > 2)
            {
                if (string.IsNullOrEmpty(ExportTableEditor.GetCellData(sheet, i, 0)))
                    break;

                data.Row++;
            }

            for (int j = 0; j < sheet.Dimension.Columns; j++)
            {
                string value = ExportTableEditor.GetCellData(sheet, i, j);

                if (i == 0)
                {
                    data.TableHeadList.Add(value);
                }
                else if (i == 1)
                {
                    data.TypeList.Add(value);
                }
                else if (i == 2)
                {
                    data.CommentList.Add(value);
                }
                else
                {
                    if (j == 0)
                    {
                        if (j == 0 && keys.Contains(value))
                        {
                            throw new Exception($"转换表:{tablePath}[{sheetName}] 第{i}行{j}列失败! \n ID:{data.TypeList[0]} {value} 重复！");
                        }
                        keys.Add(value);
                    }
                    data.TableDatas[i - 3, j] = value;
                }
            }
        }
        return data;
    }

    private static byte[] CreatePacket(string key, string sheetName, TableData tableData)
    {
        int ColNum = 0;
        for (int i = 0; i < tableData.Column; i++)
        {
            if (tableData.TableHeadList[i].StartsWith("#") || tableData.TypeList[i].StartsWith("#"))
            {
                continue;
            }
            ColNum++;
        }

        MemoryStream fs = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(fs, Encoding.UTF8);
        try
        {
            bw.Write(tableData.Row);
            bw.Write(ColNum);

            for (int i = 0; i < tableData.Column; i++)
            {
                if (tableData.TableHeadList[i].StartsWith("#") || tableData.TypeList[i].StartsWith("#"))
                {
                    continue;
                }

                bw.Write(tableData.TypeList[i]);
            }

            for (int col = 0; col < tableData.Column; col++)
            {
                var value_type = tableData.TypeList[col];
                var head = tableData.TableHeadList[col];
                if (head.StartsWith("#") || value_type.StartsWith("#"))
                {
                    continue;
                }
                if (value_type.StartsWith("enum:"))
                {
                    try
                    {
                        WriteEnum(value_type.Split(':')[1], col, bw, tableData);
                    }
                    catch (Exception exp)
                    {
                        throw new Exception($"转换表:{key}[{sheetName}]失败!\n 属性名:{head},类型:{value_type} \n{exp}");
                    }
                }
                else if (value_type == "enum")
                {
                    try
                    {
                        WriteEnum($"E{tableData.TableHeadList[col]}", col, bw, tableData);
                    }
                    catch (Exception exp)
                    {
                        throw new Exception($"转换表:{key}[{sheetName}]失败!\n 属性名:{head},类型:{value_type} \n{exp}");
                    }
                }
                else
                {
                    for (int row = 0; row < tableData.Row; row++)
                    {
                        string value = tableData.TableDatas[row, col];
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
                                        throw new Exception("DateTime数据异常 " + tableData.TableHeadList[col] + " " + tableData.TypeList[col]);
                                    }

                                    bw.Write(value);
                                    break;
                                case "int[]":
                                    ExportTableUtils.WriteIntArray(value, bw);
                                    break;
                                case "uint[]":
                                    ExportTableUtils.WriteUintArray(value, bw);
                                    break;
                                case "float[]":
                                    ExportTableUtils.WriteFloatArray(value, bw);
                                    break;
                                case "double[]":
                                    ExportTableUtils.WriteDoubleArray(value, bw);
                                    break;
                                case "string[]":
                                    ExportTableUtils.WriteStringArray(value, bw);
                                    break;
                                case "int[][]":
                                    ExportTableUtils.WriteIntArray2(value, bw);
                                    break;
                                case "uint[][]":
                                    ExportTableUtils.WriteUintArray2(value, bw);
                                    break;
                                case "float[][]":
                                    ExportTableUtils.WriteFloatArray2(value, bw);
                                    break;
                                case "double[][]":
                                    ExportTableUtils.WriteDoubleArray2(value, bw);
                                    break;
                                case "string[][]":
                                    ExportTableUtils.WriteStringArray2(value, bw);
                                    break;
                                default:
                                    throw new Exception("未识别的类型 = " + tableData.TableHeadList[col] + " " + tableData.TypeList[col]);
                            }
                        }
                        catch (Exception exp)
                        {
                            throw new Exception($"转换表:{key}[{sheetName}]失败!\n第{row}行{col}列, 属性名:{head},类型:{value_type},值:{value}\n{exp}");
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

    private static void WriteEnum(string type, int i, BinaryWriter bw, TableData tableData)
    {
        if (ExportTableUtils.enumTypes.TryGetValue(type, out var enum_type))
        {
            for (int j = 0; j < tableData.Row; j++)
            {
                var enum_value = tableData.TableDatas[j, i];
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
            throw new Exception("未识别的类型 = " + tableData.TableHeadList[i] + " " + tableData.TypeList[i]);
        }
    }

    private static byte[] CreateCSFile(string tableName, string sheetName, string className, string dataName, string packetName, TableData tableData)
    {
        var fs = new MemoryStream();
        var sw = new StreamWriter(fs, Encoding.UTF8);

        sw.Write("using System;" + "\n");
        sw.Write("using System.Collections.Generic;" + "\n");
        sw.Write("using UnityEngine;" + "\n");
        sw.Write("\n");
        sw.Write("public partial class " + className + " : TableData" + "\n");
        sw.Write("{" + "\n");
        sw.Write("\t" + "public readonly string sFilePath = " + "\"" + dataName + "\";" + "\n");
        sw.Write("\t" + "public Dictionary<" + tableData.TypeList[0] + ", tData> mData;" + "\n");
        sw.Write("\t" + "public List<tData> DataList;" + "\n");
        sw.Write("\t" + "public partial struct tData" + "\n");
        sw.Write("\t" + "{" + "\n");
        for (int i = 0; i < tableData.Column; i++)
        {
            if (tableData.TableHeadList[i].StartsWith("#") || tableData.TypeList[i].StartsWith("#"))
            {
                continue;
            }
            if (tableData.CommentList[i] != null && tableData.CommentList[i] != "")
            {
                sw.Write("\t\t" + "/// <summary>" + "\n");

                if (tableData.CommentList[i].Contains("\r\n"))
                {
                    string[] arr = tableData.CommentList[i].Split("\r\n");

                    foreach (string str in arr)
                    {
                        sw.Write("\t\t" + "///" + str + "\n");
                    }
                }
                else if (tableData.CommentList[i].Contains("\r"))
                {
                    string[] arr = tableData.CommentList[i].Split("\r");

                    foreach (string str in arr)
                    {
                        sw.Write("\t\t" + "///" + str + "\n");
                    }
                }
                else if (tableData.CommentList[i].Contains("\n"))
                {
                    string[] arr = tableData.CommentList[i].Split("\n");

                    foreach (string str in arr)
                    {
                        sw.Write("\t\t" + "///" + str + "\n");
                    }
                }
                else
                {
                    sw.Write("\t\t" + "///" + tableData.CommentList[i] + "\n");
                }

                sw.Write("\t\t" + "/// </summary>" + "\n");
            }

            if (tableData.TypeList[i] == "enum")
            {
                var enum_type = $"E{tableData.TableHeadList[i]}";
                sw.Write("\t\t" + "public " + enum_type + " " + tableData.TableHeadList[i] + ";\n");
            }
            else if (tableData.TypeList[i].StartsWith("enum:"))
            {
                var enumType = tableData.TypeList[i].Split(':');
                sw.Write("\t\t" + "public " + enumType[1] + " " + tableData.TableHeadList[i] + ";\n");
            }
            else
            {
                sw.Write("\t\t" + "public " + tableData.TypeList[i] + " " + tableData.TableHeadList[i] + ";\n");
            }
        }

        sw.Write("\t\tpublic bool IsNull\n");
        sw.Write("\t\t{\n");
        sw.Write("\t\t\tget \n");
        sw.Write("\t\t\t{\n");
        if (tableData.TypeList[0].ToLower() != "string")
        {
            sw.Write($"\t\t\t\treturn {tableData.TableHeadList[0]} <= 0;\n");
        }
        else
        {
            sw.Write($"\t\t\t\treturn string.IsNullOrEmpty({tableData.TableHeadList[0]});\n");
        }
        sw.Write("\t\t\t}\n");
        sw.Write("\t\t}\n");

        sw.Write("\t" + "}" + "\n\n");
        sw.Write($"\tpublic {className}()\n");
        sw.Write("\t{\n");
        sw.Write("\t\ttry\n");
        sw.Write("\t\t{\n");
        sw.Write("\t\t\tPACKET_NAME = \"" + packetName + "\";\n");
        sw.Write("\t\t\tReadTable();\n");
        sw.Write("\t\t\tParseData();\n");
        sw.Write("\t\t}\n");
        sw.Write("\t\tcatch(Exception exp)\n");
        sw.Write("\t\t{\n");
        sw.Write("\t\t\tDebug.Log($\"LOAD TABLE ERROR!{exp}\");\n");
        sw.Write("\t\t}\n");
        sw.Write("\t}\n\n");

        sw.Write("\t" + "void ReadTable()" + "\n");
        sw.Write("\t" + "{" + "\n");
        sw.Write("\t\t" + "if (mTableData == null)" + "\n");
        sw.Write("\t\t" + "{" + "\n");
        sw.Write("\t\t\t" + "mTableData = new RawTable();" + "\n");
        sw.Write("\t\t" + "}" + "\n");
        sw.Write("\t\t" + $"mTableData.ReadBinary(sFilePath, PACKET_NAME);" + "\n");
        sw.Write("\t" + "}" + "\n");
        sw.Write("\t" + "void ParseData()" + "\n");
        sw.Write("\t" + "{" + "\n");
        sw.Write("\t\t" + "mData = new Dictionary<" + tableData.TypeList[0] + ", tData>(mTableData._nRows);" + "\n");
        sw.Write("\t\t" + "DataList = new List<tData>(mTableData._nRows);" + "\n");
        sw.Write("\t\t" + "for (int i = 0; i < mTableData._nRows; i++)" + "\n");
        sw.Write("\t\t" + "{" + "\n");
        sw.Write("\t\t\t" + "tData data = new tData();" + "\n");

        int j = 0;
        for (int i = 0; i < tableData.TableHeadList.Count; i++)
        {
            if (tableData.TableHeadList[i].StartsWith("#") || tableData.TypeList[i].StartsWith("#"))
            {
                continue;
            }
            string value_name = tableData.TableHeadList[i];
            string value_type = tableData.TypeList[i];
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
                        throw new Exception($"转换表:{tableName}[{sheetName}]失败! \n不支持的类型=>属性名:{value_name},类型:{value_type}");
                }
            }
            sw.Write("\t\t\t" + "data." + value_name + " = " + str_func + "\n");
            j++;
        }

        sw.Write("\t\t\t" + "mData.Add(data." + tableData.TableHeadList[0] + ", data);" + "\n");
        sw.Write("\t\t\t" + "DataList.Add(data);" + "\n");
        sw.Write("\t\t" + "}" + "\n");
        sw.Write("\t\t" + " mTableData = null;" + "\n");
        sw.Write("\t" + "}" + "\n");
        sw.Write("}" + "\n");

        sw.Close();
        fs.Close();

        return fs.ToArray();
    }
}
