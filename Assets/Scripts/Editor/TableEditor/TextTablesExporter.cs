using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using OfficeOpenXml;

public class TextTablesExporter
{
    string mTablesPath;
    string mOutTextCodePath;
    string mOutPacketName;
    string[][] mTextExportSettings;
    Dictionary<string, List<int>> mTextExcelSheetMap = new Dictionary<string, List<int>>();
    List<string[]> mCompileTexts = new List<string[]>();
    List<string[]> mNoCompileTexts = new List<string[]>();

    private struct TextTableParams
    {
        public List<string> TextKeys;
        public List<string[]> CompileTexts;
        public List<string[]> NoCompileTexts;
        public TextTableParams(List<string> textKeys, List<string[]> compileTexts, List<string[]> noCompileTexts)
        {
            TextKeys = textKeys;
            CompileTexts = compileTexts;
            NoCompileTexts = noCompileTexts;
        }
    }

    public TextTablesExporter(string configTableName, string textSheetName, string tablesPath)
    {
        mTablesPath = tablesPath;
        InitTextTableSettings(configTableName, textSheetName);
        ClearTextTableCreatedFiles();
    }

    private void InitTextTableSettings(string configTableName, string textSheetName)
    {
        ExcelPackage configTables = ExportTableUtil.ReadExcel(mTablesPath + "/" + Path.GetFileNameWithoutExtension(configTableName));
        if (configTables == null)
        {
            throw new Exception(mTablesPath + "/" + Path.GetFileNameWithoutExtension(configTableName) + "路径为空!");
        }
        ExcelWorksheet sheet = configTables.Workbook.Worksheets[textSheetName];
        mTextExcelSheetMap.Clear();
        mOutTextCodePath = ExportTableUtil.GetCell(sheet, 0, 1);
        mOutPacketName = ExportTableUtil.GetCell(sheet, 1, 1);

        int RowNum = sheet.Dimension.Rows - 3;
        mTextExportSettings = new string[RowNum][];
        for (int i = 0; i < RowNum; i++)
        {
            mTextExportSettings[i] = new string[3];
            for (int j = 0; j < 3; j++)
            {
                mTextExportSettings[i][j] = ExportTableUtil.GetCell(sheet, i + 3, j);
            }

            if (!mTextExcelSheetMap.ContainsKey(mTextExportSettings[i][0]))
            {
                mTextExcelSheetMap.Add(mTextExportSettings[i][0], new List<int>());
            }
            mTextExcelSheetMap[mTextExportSettings[i][0]].Add(i);
        }
        ExportTableUtil.MaxTablesCount(mTextExcelSheetMap.Count);
    }

    private void ClearTextTableCreatedFiles()
    {
        if (File.Exists(mOutTextCodePath + ExportTableUtil.TEXT_TABLE_FILE_NAME))
        {
            File.Delete(mOutTextCodePath + ExportTableUtil.TEXT_TABLE_FILE_NAME);
        }
    }

    public void ExportTextTable()
    {
        foreach (KeyValuePair<string, List<int>> p in mTextExcelSheetMap)
        {
            LocalTablesExporter.RegisterExportTask(() =>
            {
                TextTableFileTask(p);
            }, "文本表/" + p.Key);
        }
    }

    private void TextTableFileTask(KeyValuePair<string, List<int>> input)
    {
        TextTableParams textParams = new TextTableParams(new List<string>(), new List<string[]>(), new List<string[]>());

        ParseTextData(input.Key, textParams);
        foreach (var compileText in textParams.CompileTexts)
        {
            mCompileTexts.Add(compileText);
        }
        foreach (var noCompileText in textParams.NoCompileTexts)
        {
            mNoCompileTexts.Add(noCompileText);
        }
    }

    public void ExportCSCode()
    {
        ExportTextCSParse(mOutTextCodePath + ExportTableUtil.TEXT_TABLE_FILE_NAME, mCompileTexts);

        PacketEditor.Create(mOutPacketName);
        for (var i = 0; i < ExportTableUtil.TEXT_HEAD.Length; i++)
        {
            PacketEditor.AddFile(mOutPacketName, ExportTableUtil.TEXT_HEAD[i] + ".bytes", ExportTextToBytes(i, mCompileTexts, mNoCompileTexts));
        }
    }

    private void ParseTextData(string excel_name, TextTableParams textParams)
    {
        string file_path = mTablesPath + "/" + excel_name;

        ExcelPackage table = ExportTableUtil.ReadExcel(file_path);

        List<int> sheetList = mTextExcelSheetMap[excel_name];
        foreach (int i in sheetList)
        {
            string[] exportSetting = mTextExportSettings[i];
            if (string.IsNullOrEmpty(exportSetting[0]) || string.IsNullOrEmpty(exportSetting[1]) || string.IsNullOrEmpty(exportSetting[2]))
            {
                throw new Exception("TablesConfig配置错误 行数 = " + i);
            }
            string sheet_name = exportSetting[1];
            bool isCompile = Convert.ToBoolean(exportSetting[2]);

            ExcelWorksheet sheet = table.Workbook.Worksheets[sheet_name];

            if (sheet == null)
            {
                throw new Exception($"表:{excel_name}[{sheet_name}]不存在!");
            }

            if (sheet.Dimension.Rows < 1)
            {
                throw new Exception($"表:{excel_name}[{sheet_name}]格式不正确.行数应该大于4.");
            }

            for (int row_num = 1; row_num < sheet.Dimension.Rows; row_num++)
            {
                var key = ExportTableUtil.GetCell(sheet, row_num, 0);
                if (!string.IsNullOrEmpty(key))
                {
                    if (isCompile && !ExportTableUtil.CheckHead(key, textParams.TextKeys))
                    {
                        throw new Exception($"转换表:{excel_name}[{sheet_name}] 第{row_num}行失败! \nID{key}是 非法（字母开头只能用字母数字下划线）或 重复 或 保留关键字");
                    }

                    if (!textParams.TextKeys.Contains(key))
                    {
                        textParams.TextKeys.Add(key);
                    }
                    else
                    {
                        throw new Exception($"转换表:{excel_name}[{sheet_name}] 第{row_num}行失败! \nID{key}重复");
                    }

                    var data = new string[ExportTableUtil.TEXT_HEAD.Length + 1];
                    data[0] = key;
                    for (var j = 0; j < ExportTableUtil.TEXT_HEAD.Length; j++)
                    {
                        data[j + 1] = ExportTableUtil.GetCell(sheet, row_num, j + 1);
                    }
                    if (isCompile)
                    {
                        textParams.CompileTexts.Add(data);
                    }
                    else
                    {
                        textParams.NoCompileTexts.Add(data);
                    }
                }
                else
                {
                    throw new Exception($"转换表:{excel_name}[{sheet_name}] 第{row_num}行失败! \nID为空");
                }
            }
        }
    }

    private void ExportTextCSParse(string code_file_path, List<string[]> compileTexts)
    {
        using (var stream = new FileStream(code_file_path, FileMode.Create))
        {
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                writer.Write("public static partial class TEXT\n");
                writer.Write("{\n");
                for (var i = 0; i < compileTexts.Count; i++)
                {
                    var key = compileTexts[i][0];

                    WriteSummary(writer, compileTexts[i][1], (string)compileTexts[i][2]);

                    writer.Write("\tpublic static string " + key + "\n");
                    writer.Write("\t{\n");
                    writer.Write("\t\tget { return GetText(\"" + key + "\"); }\n");
                    writer.Write("\t}\n");
                }
                writer.Write("}");
                writer.Close();
            }
            stream.Close();
        }
    }

    private void WriteSummary(StreamWriter writer, string cn, string en)
    {
        cn = cn.Replace("\n", "\n\t///");
        en = en.Replace("\n", "\n\t///");

        writer.Write($"\t/// <summary>");
        writer.Write("\n");
        writer.Write($"\t/// cn: {cn}");
        writer.Write("\n");
        writer.Write($"\t/// en: {en}");
        writer.Write("\n");
        writer.Write($"\t/// </summary>");
        writer.Write("\n");
    }

    private byte[] ExportTextToBytes(int languageIndex, List<string[]> compileTexts, List<string[]> noCompileTexts)
    {
        var data = new MemoryStream();
        using (var writer = new BinaryWriter(data, Encoding.UTF8))
        {
            for (var i = 0; i < compileTexts.Count; i++)
            {
                var key = compileTexts[i][0];
                var text = compileTexts[i][languageIndex + 1];

                writer.Write(key);
                writer.Write(text);
            }

            for (var i = 0; i < noCompileTexts.Count; i++)
            {
                var key = noCompileTexts[i][0];
                var text = noCompileTexts[i][languageIndex + 1];

                writer.Write(key);
                writer.Write(text);
            }

            writer.Close();
        }
        return data.ToArray();
    }
}
