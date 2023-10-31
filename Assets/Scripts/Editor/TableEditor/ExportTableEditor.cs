using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public struct ExcelData
{
    public string tableName;
    public string sheetName;
    public string outputPath;
    public string className;
}


public static class ExportTableEditor
{
    const string SERVER_PATH = "ServerData/";
    private static string ConfigFileName = "Config.xlsx";
    private static string TableSheetName = "Tables";
    private static string TextSheetName = "Texts";
    private static string KeywordSheetName = "Keywords";
    private static string ConfigFilePath = "Assets/EditorAssets/Tables/";

    private static SynchronizationContext mMainThreadSynContext;
    private static List<(string tableName, long elapsedTime)> mExportList = new();
    public static event Action CompletedCallback;

    [MenuItem("导表", menuItem = "Tools/导表")]
    private static void Start()
    {
        var encrypt_path = Constent.BUILTIN_PATH;
        var unencrypt_path = SERVER_PATH;
        AssetDatabase.DisallowAutoRefresh();

        EditorApplication.CallbackFunction displayProgress = ExportTableProgress;
        EditorApplication.update += displayProgress;

        Task.Run(() =>
        {
            try
            {
                PacketEditor.UnloadAll();
                ExportTable(encrypt_path, unencrypt_path, ExportErrorCallback, ExportCompleteCallback, ExportProgressCallback);
                TABLE.UnloadAll();
                PacketEditor.UnloadAll();
                OnExportOver(true, displayProgress);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                OnExportOver(true, displayProgress);
            }
        });
    }

    private static void ExportTableProgress()
    {
        int curExportCount = ExportTableUtil.GetExportCount();
        int maxExportCount = ExportTableUtil.GetMaxTableCount();
        EditorUtility.DisplayProgressBar("导表", $"进度：{curExportCount}/{maxExportCount}", (float)curExportCount / maxExportCount);
    }

    private static void ExportTable(string encrypt_path, string unencrypt_path, Action<string, Exception> errorCallback, Action completeCallback, Action<string, long> progressCallback)
    {
        LocalTablesExporter.ExportSettings exportSettings = new LocalTablesExporter.ExportSettings(ConfigFileName, TableSheetName, TextSheetName, KeywordSheetName, ConfigFilePath, encrypt_path, unencrypt_path, errorCallback, completeCallback, progressCallback);

        Task.WaitAll(LocalTablesExporter.ExporterTables(exportSettings));
    }

    private static void ExportErrorCallback(string errorMessage, Exception exception)
    {
        mMainThreadSynContext.Send(new SendOrPostCallback((obj) =>
        {
            EditorUtility.DisplayDialog("Error", errorMessage, "确定");
        }), null);
        Debug.LogError(exception);
    }

    private static void ExportCompleteCallback()
    {
        StringBuilder exportElapsedTimeList = new StringBuilder();
        long totalDuration = 0;
        mExportList.Sort((a, b) =>
        {
            return (int)(b.elapsedTime - a.elapsedTime);
        });
        for (int i = 0; i < mExportList.Count; i++)
        {
            exportElapsedTimeList.Append($"耗时排序{i}: {mExportList[i].tableName} 导出时间: {mExportList[i].elapsedTime / 1000f}s\n");
            totalDuration += mExportList[i].elapsedTime;
        }
        exportElapsedTimeList.Append($"\n总耗时: {totalDuration / 1000f}s\n");
        Debug.Log(exportElapsedTimeList.ToString());
    }

    private static void ExportProgressCallback(string tableName, long elapsedTimeMilliseconds)
    {
        mExportList.Add(ValueTuple.Create(tableName, elapsedTimeMilliseconds));
    }

    private static void OnExportOver(bool isSuccess, EditorApplication.CallbackFunction DisplayeProgressBarAction)
    {
        mMainThreadSynContext.Send(new SendOrPostCallback((obj) =>
        {
            string title = isSuccess ? "导表成功" : "导表失败";
            EditorApplication.update -= DisplayeProgressBarAction;
            EditorUtility.DisplayProgressBar(title, "正在刷新资源目录", 1.0f);
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
            AssetDatabase.AllowAutoRefresh();
            Debug.Log(title);
            CompletedCallback?.Invoke();
        }), null);
    }
}
