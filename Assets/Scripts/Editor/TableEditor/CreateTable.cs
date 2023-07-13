using System.IO;
using UnityEditor;
using UnityEngine;
using OfficeOpenXml;



public class CreateTable
{
    [MenuItem("New Table", menuItem = "Assets/Create/配置/创建xlsx")]
    private static void Create()
    {
        string path = EditorUtility.SaveFilePanel("创建Excel", Constent.TABLE_CONFIG_PATH, "New Table.xlsx", "xlsx");

        if (string.IsNullOrEmpty(path)) return;

        var file = new FileInfo(path);
        if(file.Exists)
        {
            Debug.LogWarning("存在相同的表");
        }
        else
        {
            CreateExcel(file);
        }
    }

    private static void CreateExcel(FileInfo file)
    {
        using (var excel = new ExcelPackage(file))
        {
            excel.Workbook.Worksheets.Add("table");
            excel.Save();
            AssetDatabase.Refresh();
        }
    }
}
