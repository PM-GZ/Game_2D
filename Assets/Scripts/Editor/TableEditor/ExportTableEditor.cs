using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OfficeOpenXml;

public struct ExcelData
{
    public string tableName;
    public string sheetName;
    public string outputPath;
    public string className;
}


public class ExportTableEditor
{
    private static Dictionary<string, List<ExcelData>> _ExcelDataDict = new();
    private static StringBuilder _RepeatTableTip = new();

    [MenuItem("导表", menuItem = "Tools/导表")]
    private static void Start()
    {
        _ExcelDataDict.Clear();

        var fileInfo = new FileInfo($"{Constent.TABLE_CONFIG_PATH}/MainTable.xlsx");
        GetAllExcel(new ExcelPackage(fileInfo));

        if (_RepeatTableTip.Length > 0)
        {
            EditorUtility.DisplayDialog("错误", _RepeatTableTip.ToString(), "确认");
            return;
        }

        ReadExcels();
        AssetDatabase.Refresh();
    }

    private static void GetAllExcel(ExcelPackage excel)
    {
        var sheet = excel.Workbook.Worksheets["Tables"];
        for (int i = 2; i <= sheet.Dimension.Rows; i++)
        {
            if (TableUtility.IsRowEmpty(sheet, i, i)) continue;

            var excelData = new ExcelData()
            {
                tableName = sheet.GetValue<string>(i, 1),
                sheetName = sheet.GetValue<string>(i, 2),
                outputPath = sheet.GetValue<string>(i, 3),
                className = sheet.GetValue<string>(i, 4),
            };

            if (!_ExcelDataDict.ContainsKey(excelData.tableName))
            {
                _ExcelDataDict.Add(excelData.tableName, new());
            }
            _ExcelDataDict[excelData.tableName].Add(excelData);
        }
        excel.Dispose();
    }

    private static StringBuilder _ClassSBuilder = new();
    private static void ReadExcels()
    {
        foreach (var excel in _ExcelDataDict)
        {
            foreach (var excelData in excel.Value)
            {
                string file = GetExcel(excelData.tableName);
                if (file == null) continue;
                FileInfo fileInfo = new FileInfo(file);
                using var excelPkg = new ExcelPackage(fileInfo);

                var sheet = excelPkg.Workbook.Worksheets[excelData.sheetName];
                AddClassHead(excelData.className);
                AddClassField(excelData.sheetName, fileInfo.FullName);
                AddStructData(sheet);
                AddClassConstructor(excelData.className);
                AddClassParseDataFunc(sheet);
                _ClassSBuilder.Append("\n}");

                CreateClassFile(excelData);
                _ClassSBuilder.Length = 0;
            }
        }
    }

    private static string GetExcel(string tableName)
    {
        string[] files = Directory.GetFiles($"{Constent.TABLE_CONFIG_PATH}", $"*xlsx", SearchOption.AllDirectories);
        foreach (var item in files)
        {
            if (Equals(Path.GetFileNameWithoutExtension(item), tableName))
            {
                return item;
            }
        }
        return null;
    }

    private static void AddClassHead(string className)
    {
        _ClassSBuilder.Append($"using System;\n");
        _ClassSBuilder.Append($"using System.Collections.Generic;\n\n\n");

        _ClassSBuilder.Append($"public class {className} : TableData\n");
        _ClassSBuilder.Append("{");
        _ClassSBuilder.Append("\n\t");
    }

    private static void AddClassField(string sheetName, string filePath)
    {
        filePath = filePath.Replace('\\', '/');
        filePath = filePath.Replace(Application.dataPath, "Assets");
        _ClassSBuilder.Append($"public readonly string filePath = \"{filePath}\";");
        _ClassSBuilder.Append($"\n\tpublic readonly sheetName = \"{sheetName}\";");
        _ClassSBuilder.Append("\n\t");
        _ClassSBuilder.Append($"public Dictionary<uint, Data> dataDict;\n\n");
    }

    private static void AddStructData(ExcelWorksheet sheet)
    {
        _ClassSBuilder.Append("\n\tpublic struct Data\n\t{");
        for (int j = 1; j <= sheet.Dimension.Columns; j++)
        {
            var field = sheet.GetValue(2, j);
            var type = sheet.GetValue(3, j);
            _ClassSBuilder.Append($"\n\t\tpublic {type} {field};");
        }
        _ClassSBuilder.Append("\n\t}\n\n");
    }

    private static void AddClassConstructor(string className)
    {
        _ClassSBuilder.Append($"\tpublic {className}()\n\t{{");
        _ClassSBuilder.Append("\n\t\tif(rawTable == null)");
        _ClassSBuilder.Append("\n\t\t{");
        _ClassSBuilder.Append("\n\t\t\trawTable = new RawTable();");
        _ClassSBuilder.Append("\n\t\t}");
        _ClassSBuilder.Append("\n\t\trawTable.ReadTable(filePath, sheetName);");
        _ClassSBuilder.Append("\n\t\tParseData();");
        _ClassSBuilder.Append("\n\t}\n");
    }

    private static void AddClassParseDataFunc(ExcelWorksheet sheet)
    {
        _ClassSBuilder.Append("\n\tprivate void ParseData()");
        _ClassSBuilder.Append("\n\t{");
        _ClassSBuilder.Append("\n\t\tdataDict = new(rawTable.rowNum - 3);");
        _ClassSBuilder.Append("\n\t\tfor (int i = 0; i < rawTable.rowNum - 3; i++)\n\t\t{");
        _ClassSBuilder.Append("\n\t\t\tData data = new();\n\t\t\t");

        string key = "";
        for (int j = 1; j <= sheet.Dimension.Columns; j++)
        {
            if (j == 1)
            {
                key = sheet.GetValue(2, j).ToString();
            }
            var field = sheet.GetValue(2, j);
            var type = sheet.GetValue(3, j).ToString();

            int subIndex = type.StartsWith('u') ? 2 : 1;
            string func = type[..subIndex].ToUpper() + type[subIndex..];
            _ClassSBuilder.Append($"data.{field} = rawTable.Get{func}(i, {j - 1});\n\t\t\t");
        }
        _ClassSBuilder.Append($"dataDict.Add(data.{key}, data);");
        _ClassSBuilder.Append("\n\t\t}");
        _ClassSBuilder.Append("\n\t\trawTable = null;");
        _ClassSBuilder.Append("\n\t}");
    }

    private static void CreateClassFile(ExcelData excelData)
    {
        string path = $"{excelData.outputPath}";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        File.WriteAllText($"{path}{excelData.className}.cs", _ClassSBuilder.ToString());
    }
}
