using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class TextTableExport
{
    private struct TableData
    {
        public List<string> KeyList;
        public List<string[]> CompileTexts;
        public List<string[]> NoCompileTexts;

        public TableData(List<string> headList, List<string[]> keyList, List<string[]> languageList)
        {
            KeyList = headList;
            CompileTexts = keyList;
            NoCompileTexts = languageList;
        }
    }

    private string OutCodePath;
    private string PacketName;
    private string[] LANGUAGES = { "CN", "EN" };
    private string mTablePath;
    private ExcelWorksheet mTable;
    private string[,] mConfigTableDatas;
    private Dictionary<string, List<int>> mSheetPathDict;


    List<string[]> compileTable = new List<string[]>();
    List<string[]> noCompileTable = new List<string[]>();

    public TextTableExport(string tablePath, string sheetName, ExcelPackage config)
    {
        mTablePath = tablePath;
        mTable = config.Workbook.Worksheets[sheetName];

        OutCodePath = ExportTableEditor.GetCellData(mTable, 0, 1);
        PacketName = ExportTableEditor.GetCellData(mTable, 1, 1);

        GetAllSheet();
        DeleteOldFiles();
    }

    private void GetAllSheet()
    {
        mSheetPathDict = new Dictionary<string, List<int>>();
        mConfigTableDatas = new string[mTable.Dimension.Rows - 3, mTable.Dimension.Columns];
        for (int i = 0; i < mTable.Dimension.Rows - 3; i++)
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

            if (!mSheetPathDict.ContainsKey(mConfigTableDatas[i, 0]))
            {
                mSheetPathDict.Add(mConfigTableDatas[i, 0], new());
            }
            mSheetPathDict[mConfigTableDatas[i, 0]].Add(i);
        }
        ExportTableEditor.SetMaxCount(mSheetPathDict.Count);
    }

    private void DeleteOldFiles()
    {
        string path = $"{mTablePath}TEXT_Keys.cs";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public void ExportData()
    {
        foreach (var dict in mSheetPathDict)
        {
            ExportTableEditor.RegisterExportTask(() =>
            {
                TableData tableData = GetTableData(dict.Key);

                foreach (var text in tableData.CompileTexts)
                {
                    compileTable.Add(text);
                }
                foreach (var text in tableData.NoCompileTexts)
                {
                    noCompileTable.Add(text);
                }
            }, dict.Key);
        }
    }

    private TableData GetTableData(string excelName)
    {
        string filePath = mTablePath + excelName;
        var table = ExportTableEditor.GetConfigFile(filePath);
        var sheetList = mSheetPathDict[excelName];
        TableData tableData = new TableData(new(), new(), new());

        foreach (var i in sheetList)
        {
            string sheetName = mConfigTableDatas[i, 1];
            bool isCompile = Convert.ToBoolean(mConfigTableDatas[i, 2]);
            var sheet = table.Workbook.Worksheets[sheetName];
            if (sheet == null)
            {
                throw new Exception($"{excelName}中找不到：{sheetName}");
            }
            if (sheet.Dimension.Rows < 1)
            {
                throw new Exception("文件格式不对:" + excelName + " Sheet:" + sheetName);
            }

            for (int row = 1; row < sheet.Dimension.Rows; row++)
            {
                string key = ExportTableEditor.GetCellData(sheet, row, 0);
                if (string.IsNullOrEmpty(key))
                {
                    throw new Exception($"转换表:{excelName}[{sheetName}] 第{row}行失败! \nID为空");
                }
                else
                {
                    var data = new string[LANGUAGES.Length + 1];
                    data[0] = key;
                    for (int j = 0; j < LANGUAGES.Length; j++)
                    {
                        string value = ExportTableEditor.GetCellData(sheet, row, j + 1);
                        data[j + 1] = value;
                    }
                    if (isCompile)
                    {
                        tableData.CompileTexts.Add(data);
                    }
                    else
                    {
                        tableData.NoCompileTexts.Add(data);
                    }
                }
            }
        }

        return tableData;
    }

    private void CreatePacket()
    {
        for (int i = 0; i < LANGUAGES.Length; i++)
        {
            byte[] data = CreatePacket(i);
            PacketUtils.AddFile(PacketName, LANGUAGES[i] + ".bytes", data);
        }
    }

    private byte[] CreatePacket(int languageIndex)
    {
        MemoryStream fs = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(fs, Encoding.UTF8);

        for (int i = 0; i < compileTable.Count; i++)
        {
            string key = compileTable[i][0];
            string value = compileTable[i][languageIndex + 1];

            bw.Write(key);
            bw.Write(value);
        }

        for (int i = 0; i < noCompileTable.Count; i++)
        {
            string key = noCompileTable[i][0];
            string value = noCompileTable[i][languageIndex + 1];

            bw.Write(key);
            bw.Write(value);
        }

        fs.Close();
        bw.Close();
        return fs.ToArray();
    }

    public void CreateAll()
    {
        CreatePacket();
        CreateCSFile();
    }

    private void CreateCSFile()
    {

        var fs = new MemoryStream();
        var sw = new StreamWriter(fs, Encoding.UTF8);

        sw.Write("public static class TEXT_Keys\n");
        sw.Write("{\n");
        for (int i = 0; i < compileTable.Count; i++)
        {
            string key = compileTable[i][0];
            string cn = compileTable[i][1].Replace("\n", "\n\t///");
            string en = compileTable[i][2].Replace("\n", "\n\t///");

            sw.Write($"\t/// <summary>");
            sw.Write("\n");
            sw.Write($"\t/// cn: {cn}");
            sw.Write("\n");
            sw.Write($"\t/// en: {en}");
            sw.Write("\n");
            sw.Write($"\t/// </summary>");
            sw.Write("\n");

            sw.Write("\tpublic static string " + key + "\n");
            sw.Write("\t{\n");
            sw.Write("\t\tget { return TEXT.GetText(\"" + key + "\"); }\n");
            sw.Write("\t}\n");
        }
        sw.Write("}\n");


        sw.Close();
        fs.Close();
        File.WriteAllBytes($"{OutCodePath}TEXT_Keys.cs", fs.ToArray());
    }
}
