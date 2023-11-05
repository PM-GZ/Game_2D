using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using OfficeOpenXml;



public class ExportTableEditor
{
    private const string ConfigFilePath = "Assets/EditorAssets/Tables/";
    private const string ConfigName = "Config";
    private const string TableName = "Tables";
    private const string TextSheetName = "Texts";
    private const string KeywordSheetName = "Keyword";

    private static SynchronizationContext mMainThread;
    private static ExcelPackage mConfig;

    [MenuItem("Tools/导表")]
    private static void ExportTable()
    {
        AssetDatabase.DisallowAutoRefresh();

        //mMainThread = SynchronizationContext.Current;
        //EditorApplication.CallbackFunction progressCallback = DisplayeProgressBarAction;
        //EditorApplication.update += progressCallback;

        StartExportTable();
        AssetDatabase.Refresh();

        AssetDatabase.AllowAutoRefresh();
    }

    private static void DisplayeProgressBarAction()
    {

    }

    private static void StartExportTable()
    {
        mConfig = GetConfigFile(ConfigFilePath + ConfigName);
        DataTableExport.ExportDataTable(ConfigFilePath, mConfig);
        TextTableExport.ExportTextTable(ConfigFilePath, mConfig);
        PacketUtils.BuildAll();
    }

    public static ExcelPackage GetConfigFile(string tablePath)
    {
        string path = tablePath + ".xlsx";
        if (!File.Exists(path))
        {
            throw new System.Exception($"文件不存在：{path}");
        }
        try
        {
            using Stream stream = File.OpenRead(path);
            return new ExcelPackage(stream);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public static string GetCellData(ExcelWorksheet sheet, int row, int col)
    {
        try
        {
            string value = sheet.GetValue<string>(row + 1, col + 1);
            if (string.IsNullOrEmpty(value))
            {
                value = string.Empty;
            }
            return value;
        }
        catch (Exception e)
        {
            throw e;
        }
    }
}
