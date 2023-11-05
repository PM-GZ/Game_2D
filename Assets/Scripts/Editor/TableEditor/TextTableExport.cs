using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public static class TextTableExport
{
    private struct TableData
    {
        public Dictionary<string, LanguageData> TableDatas;
        public int Row;
        public int Column;

        public TableData(Dictionary<string, LanguageData> tableDatas, int row, int column)
        {
            TableDatas = tableDatas;
            Row = row;
            Column = column;
        }
    }

    private struct LanguageData
    {
        public string CN;
        public string EN;
    }

    private static string PacketName = "Language.p";
    private static string OutCodePath = "Assets/Scripts/Game/Tables/";
    private static string mTablePath;
    private static ExcelWorksheet mTable;
    private static string[,] mConfigTableDatas;
    private static Dictionary<string, List<string>> mSheetPathDict;



    public static void ExportTextTable(string tablePath, ExcelPackage config)
    {
        mTablePath = tablePath;
        mTable = config.Workbook.Worksheets["Texts"];

        GetAllSheet();
        DeleteOldFiles();
        ExportData();
    }

    private static void GetAllSheet()
    {
        mSheetPathDict = new Dictionary<string, List<string>>();
        mConfigTableDatas = new string[mTable.Dimension.Rows - 3, mTable.Dimension.Columns];
        for (int i = 0; i < mTable.Dimension.Rows; i++)
        {
            for (int j = 0; j < mTable.Dimension.Columns; j++)
            {
                string value = ExportTableEditor.GetCellData(mTable, i + 3, j);
                if (j == 0 && string.IsNullOrEmpty(value))
                {
                    return;
                }

                mConfigTableDatas[i, j] = value;
            }

            if (mConfigTableDatas[i, 2].ToLower().Equals("true"))
            {
                string sheetPath = $"{mTablePath}{mConfigTableDatas[i, 0]}";
                if (!mSheetPathDict.ContainsKey(sheetPath))
                {
                    mSheetPathDict.Add(sheetPath, new());
                }
                mSheetPathDict[sheetPath].Add(mConfigTableDatas[i, 1]);
            }
        }
    }

    private static void DeleteOldFiles()
    {
        string path = $"{mTablePath}TEXT_Keys.cs";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private static void ExportData()
    {
        int index = 0;
        foreach (var dict in mSheetPathDict)
        {
            foreach (var item in dict.Value)
            {
                TableData tableData = GetTableData(dict.Key, item);
                byte[] packetData = CreatePacket(tableData);
                byte[] csData = CreateCSFile(tableData);
                PacketUtils.AddFile(PacketName, packetData);
                File.WriteAllBytes($"{OutCodePath}TEXT_Keys.cs", csData);
                index++;
            }
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
        if (sheet.Dimension.Rows < 1)
        {
            throw new Exception("文件格式不对:" + tablePath + " Sheet:" + sheetName);
        }

        List<string> repeatKeys = new List<string>();
        TableData data = new TableData(new(), 0, 0);
        data.Column = sheet.Dimension.Columns;
        for (int i = 0; i < sheet.Dimension.Rows; i++)
        {
            if (i > 1)
            {
                if (string.IsNullOrEmpty(ExportTableEditor.GetCellData(sheet, i + 1, 0)))
                    break;

                data.Row++;
            }

            string key = string.Empty;
            var languageData = new LanguageData();
            for (int j = 0; j < sheet.Dimension.Columns; j++)
            {
                string value = ExportTableEditor.GetCellData(sheet, i + 1, j);
                if (j == 0)
                {
                    if (repeatKeys.Contains(value))
                    {
                        throw new Exception($"转换表:{tablePath}[{sheetName}] 第{i}行{j}列失败! \n ID: {value} 重复！");
                    }
                    repeatKeys.Add(value);
                    key = value;
                }
                else if (j == 1)
                {
                    languageData.CN = value;
                }
                else if (j == 2)
                {
                    languageData.EN = value;
                }
            }
            data.TableDatas.Add(key, languageData);
        }
        return data;
    }

    private static byte[] CreatePacket(TableData tableData)
    {
        int ColNum = tableData.Column;

        MemoryStream fs = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(fs, Encoding.UTF8);
        try
        {
            bw.Write(tableData.Row);
            bw.Write(ColNum);

            foreach (var item in tableData.TableDatas)
            {
                bw.Write(Convert.ToString(item.Key));
                bw.Write(Convert.ToString(item.Value.CN));
                bw.Write(Convert.ToString(item.Value.EN));
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        fs.Close();
        bw.Close();
        return fs.ToArray();
    }

    private static byte[] CreateCSFile(TableData tableData)
    {
        var fs = new MemoryStream();
        var sw = new StreamWriter(fs, Encoding.UTF8);


        sw.Write("public static class TEXT_Keys\n");
        sw.Write("{\n");
        foreach (var item in tableData.TableDatas)
        {
            sw.Write($"public static string {item.Key} {{ get => TEXT.GetText(\"{item.Key}\"); }}\n");
        }
        sw.Write("}\n");


        sw.Close();
        fs.Close();
        return fs.ToArray();
    }
}
