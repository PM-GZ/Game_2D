using System.IO;
using UnityEditor;
using OfficeOpenXml;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public struct ExcelData
{
    public string tableName;
    public string sheetName;
    public string outputPath;
    public string className;
}


public class ExportTableEditor
{
    private static Dictionary<string, ExcelData> _ExcelDataDict = new();
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
    }

    private static void GetAllExcel(ExcelPackage excel)
    {
        var sheet = excel.Workbook.Worksheets["Tables"];
        for (int i = 2; i <= sheet.Dimension.Rows; i++)
        {
            if (IsRowEmpty(sheet, i, 1, i)) continue;

            var excelData = new ExcelData()
            {
                tableName = sheet.GetValue<string>(i, 1),
                sheetName = sheet.GetValue<string>(i, 2),
                outputPath = sheet.GetValue<string>(i, 3),
                className = sheet.GetValue<string>(i, 4),
            };

            if (!_ExcelDataDict.TryAdd(excelData.tableName, excelData))
            {
                _RepeatTableTip.Append($"表：{excelData.tableName} 重复！！！\n");
            }
        }
        excel.Dispose();
    }

    private static StringBuilder _ClassSBuilder = new();
    private static void ReadExcels()
    {
        foreach (var excelData in _ExcelDataDict)
        {
            FileInfo fileInfo = new FileInfo($"{Constent.TABLE_CONFIG_PATH}/{excelData.Value.tableName}.xlsx");
            using (var excel = new ExcelPackage(fileInfo))
            {
                foreach (var sheet in excel.Workbook.Worksheets)
                {
                    _ClassSBuilder.Clear();
                    AddClassHead(excelData.Value);
                    ReadRowAndCol(sheet);
                }
            }
        }
    }

    private static void AddClassHead(ExcelData excelData)
    {
        _ClassSBuilder.Append($"public class {excelData.className}\n");
        _ClassSBuilder.Append(@"{");
    }

    private static void ReadRowAndCol(ExcelWorksheet sheet)
    {
        var fieldType = GetCellsValue(sheet, 2);
        for (int i = 3; i <= sheet.Dimension.Rows; i++)
        {
            if (IsRowEmpty(sheet, i, 1, i)) continue;
            var valueDict = GetCellsValue(sheet, i);
            foreach (var field in fieldType)
            {
                CreateTableClass(field.Value, valueDict[field.Key]);
            }
        }
    }

    private static Dictionary<int, string> GetCellsValue(ExcelWorksheet sheet, int i)
    {
        Dictionary<int, string> values = new();
        for (int j = 1; j <= sheet.Dimension.Columns; j++)
        {
            values.Add(j, sheet.GetValue<string>(i, j));
        }
        return values;
    }

    private static void CreateTableClass(string type, string value)
    {
        switch (type)
        {
            case "byte":
                break;
            case "ubyte":
                break;
            case "short":
                break;
            case "ushort":
                break;
            case "int":
                break;
            case "uint":
                break;
            case "long":
                break;
            case "ulong":
                break;
            case "float":
                break;
            case "double":
                break;
            case "char":
                break;
            case "string":
                break;
        }
    }

    private static bool IsRowEmpty(ExcelWorksheet sheet, int startRow, int startCol, int endRow)
    {
        int endCol = sheet.Cells.End.Column;
        return sheet.Cells[startRow, startCol, endRow, endCol].All(c => c.Value == null);
    }
}
