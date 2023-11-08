using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
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
    private static List<Task> mTaskList = new();

    private static int mMaxCount;
    private static int mCurCount;
    private static List<(string tableName, long elapsedTime)> mExportList = new();

    [MenuItem("Tools/导表")]
    private static void ExportTable()
    {
        mMaxCount = mCurCount = 0;
        mExportList.Clear();

        AssetDatabase.DisallowAutoRefresh();
        mMainThread = SynchronizationContext.Current;
        EditorApplication.update += DisplayeProgressBarAction;

        Task.Run(() =>
        {
            try
            {
                StartExportTable();

                mMainThread.Send(OnExportOver, null);
            }
            catch (Exception e)
            {
                throw e;
            }
        });
    }

    private static void DisplayeProgressBarAction()
    {
        EditorUtility.DisplayProgressBar("正在导表", $"正在导出数据({mCurCount}/{mMaxCount})", (float)mCurCount / mMaxCount);
    }

    private static void StartExportTable()
    {
        mConfig = GetConfigFile(ConfigFilePath + ConfigName);
        Task.WaitAll(StartExportTables());
    }

    private static void ExportProgressCallback(string tableName, long elapsedTimeMilliseconds)
    {
        mExportList.Add(ValueTuple.Create<string, long>(tableName, elapsedTimeMilliseconds));
    }

    private static void OnExportOver(object state)
    {
        EditorApplication.update -= DisplayeProgressBarAction;
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
        AssetDatabase.AllowAutoRefresh();
        OutputExportTime();
    }

    private static void OutputExportTime()
    {
        mExportList.Sort((l, r) =>
        {
            if (l.elapsedTime > r.elapsedTime) return -1;
            if (l.elapsedTime < r.elapsedTime) return 1;
            return 0;
        });

        long totalTime = 0;
        foreach (var item in mExportList)
        {
            totalTime += item.elapsedTime;
        }
        UnityEngine.Debug.Log($"导表用时排序：");
        UnityEngine.Debug.Log($"表数量：{mExportList.Count} ---- 总耗时：{totalTime / 1000f}s");
        foreach (var item in mExportList)
        {
            UnityEngine.Debug.Log($"{item.tableName} ---- {item.elapsedTime / 1000f}s");
        }
    }

    private static Task StartExportTables()
    {
        return Task.Run(() =>
        {
            try
            {
                PacketUtils.ClearAll();

                var dataTable = new DataTableExport(ConfigFilePath, TableName, mConfig);
                var textTable = new TextTableExport(ConfigFilePath, TextSheetName, mConfig);

                dataTable.ExportData();
                textTable.ExportData();

                Task.WaitAll(mTaskList.ToArray());
                textTable.CreateAll();
                PacketUtils.BuildAll();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                mTaskList.Clear();
            }
        });
    }

    public static void RegisterExportTask(Action action, string tableKey)
    {
        mTaskList.Add(Task.Run(() =>
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            action.Invoke();
            stopWatch.Stop();
            ExportProgressCallback(tableKey, stopWatch.ElapsedMilliseconds / 10);
            mCurCount++;
        }));
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

    public static void SetMaxCount(int count)
    {
        mMaxCount += count;
    }
}
