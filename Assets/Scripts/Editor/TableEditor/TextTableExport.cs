using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public static class TextTableExport
{
    private struct TableData
    {
        public List<string> HeadList;
        public List<string> KeyList;
        public List<string> LanguageList;
        public int Row;
        public int Column;

        public TableData(List<string> headList, List<string> keyList, List<string> languageList, int row, int column)
        {
            HeadList = headList;
            KeyList = keyList;
            LanguageList = languageList;
            Row = row;
            Column = column;
        }
    }

    private static string OutCodePath;
    private static string PacketName;
    private static string[] LANGUAGES = { "CN", "EN" };
    private static string mTablePath;
    private static ExcelWorksheet mTable;
    private static string[,] mConfigTableDatas;
    private static Dictionary<string, List<string>> mSheetPathDict;



    public static void ExportTextTable(string tablePath, ExcelPackage config)
    {
        mTablePath = tablePath;
        mTable = config.Workbook.Worksheets["Texts"];

        OutCodePath = ExportTableEditor.GetCellData(mTable, 0, 1);
        PacketName = ExportTableEditor.GetCellData(mTable, 1, 1);

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

            string sheetPath = $"{mTablePath}{mConfigTableDatas[i, 0]}";
            if (!mSheetPathDict.ContainsKey(sheetPath))
            {
                mSheetPathDict.Add(sheetPath, new());
            }
            mSheetPathDict[sheetPath].Add(mConfigTableDatas[i, 1]);
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
        List<TableData> compileTable = new List<TableData>();
        List<TableData> noCompileTable = new List<TableData>();
        foreach (var dict in mSheetPathDict)
        {
            foreach (var item in dict.Value)
            {
                TableData tableData = GetTableData(dict.Key, item);
                if (tableData.Row == 0) continue;

                if (mConfigTableDatas[index, 2].ToLower().Equals("true"))
                {
                    compileTable.Add(tableData);
                }
                else
                {
                    noCompileTable.Add(tableData);
                }

                index++;
            }
        }

        CreatePacket(compileTable, noCompileTable);

        byte[] csData = CreateCSFile(compileTable);
        File.WriteAllBytes($"{OutCodePath}TEXT_Keys.cs", csData);
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
        TableData data = new TableData(new(), new(), new(), 0, 0);
        data.Column = sheet.Dimension.Columns;

        for (int i = 0; i < sheet.Dimension.Rows; i++)
        {
            if (i > 0)
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
                    if (j > 0)
                    {
                        data.HeadList.Add(value);
                    }
                }
                else
                {
                    if (j == 0)
                    {
                        if (repeatKeys.Contains(value))
                        {
                            throw new Exception($"转换表:{tablePath}[{sheetName}] 第{i}行{j}列失败! \n ID: {value} 重复！");
                        }
                        repeatKeys.Add(value);
                        data.KeyList.Add(value);
                    }
                    else
                    {
                        data.LanguageList.Add(value);
                    }
                }
            }
        }
        return data;
    }

    private static void CreatePacket(List<TableData> compileTable, List<TableData> noCompileTable)
    {
        for (int i = 0; i < LANGUAGES.Length; i++)
        {
            byte[] data = CreatePacket(i, compileTable, noCompileTable);
            PacketUtils.AddFile(PacketName, LANGUAGES[i] + ".bytes", data);
        }
    }

    private static byte[] CreatePacket(int languageIndex, List<TableData> compileTable, List<TableData> noCompileTable)
    {
        MemoryStream fs = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(fs, Encoding.UTF8);

        for (int i = 0; i < compileTable.Count; i++)
        {
            string key = compileTable[i].KeyList[languageIndex];
            string value = compileTable[i].LanguageList[languageIndex];

            bw.Write(key);
            bw.Write(value);
        }

        for (int i = 0; i < noCompileTable.Count; i++)
        {
            string key = noCompileTable[i].KeyList[languageIndex];
            string value = noCompileTable[i].LanguageList[languageIndex];

            bw.Write(key);
            bw.Write(value);
        }

        fs.Close();
        bw.Close();
        return fs.ToArray();
    }

    private static byte[] CreateCSFile(List<TableData> tableDatas)
    {
        var fs = new MemoryStream();
        var sw = new StreamWriter(fs, Encoding.UTF8);


        sw.Write("public static class TEXT_Keys\n");
        sw.Write("{\n");
        foreach (var data in tableDatas)
        {
            foreach (var key in data.KeyList)
            {
                sw.Write($"\t/// <summary>\n");
                for (int i = 0; i < data.HeadList.Count; i++)
                {
                    sw.Write($"\t/// {data.HeadList[i]}: {data.LanguageList[i]}\n");
                }
                sw.Write($"\t/// </summary>\n");
                sw.Write($"\tpublic static string {key} {{ get => TEXT.GetText(\"{key}\"); }}\n");
            }
        }
        sw.Write("}\n");


        sw.Close();
        fs.Close();
        return fs.ToArray();
    }
}
