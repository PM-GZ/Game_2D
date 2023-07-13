using System.IO;
using UnityEditor;
using UnityEngine;
using OfficeOpenXml;



public class CreateTable
{
    [MenuItem("New Table", menuItem = "Assets/Create/����/����xlsx")]
    private static void Create()
    {
        string path = EditorUtility.SaveFilePanel("����Excel", Constent.TABLE_CONFIG_PATH, "New Table.xlsx", "xlsx");

        if (string.IsNullOrEmpty(path)) return;

        var file = new FileInfo(path);
        if(file.Exists)
        {
            Debug.LogWarning("������ͬ�ı�");
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
