using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class LocalTablesExporter
{

    public struct ExportSettings
    {
        public string ConfigTableName;
        public string TableSheetName;
        public string TextSheetName;
        public string KeywordSheetName;
        public string TablesPath;
        public string EncryPath;
        public string NoEncryPath;
        public Action<string, Exception> ErrorCallback;
        public Action CompleteCallback;
        public Action<string, long> ProgressCallback;

        public ExportSettings(string configTableName, string tableSheetName, string textSheetName, string keywordSheetName, string tablesPath, string noEncryPath, string encryPath, Action<string, Exception> errorCallback, Action completeCallback, Action<string, long> progressCallback)
        {
            ConfigTableName = configTableName;
            TableSheetName = tableSheetName;
            TextSheetName = textSheetName;
            KeywordSheetName = keywordSheetName;
            TablesPath = tablesPath;
            EncryPath = encryPath;
            NoEncryPath = noEncryPath;
            this.ErrorCallback = errorCallback;
            this.CompleteCallback = completeCallback;
            this.ProgressCallback = progressCallback;
        }
    }


    static List<Task> mTaskList = new List<Task>();
    static Action<string, Exception> mErrorCallback;
    static Action<string, long> mProgressCallback;

    public static Task ExporterTables(ExportSettings exportSettings)
    {
        return Task.Run(() =>
        {
            if(Path.GetExtension(exportSettings.ConfigTableName) != ".xlsx")
            {
                mErrorCallback.Invoke("只支持xlsx格式表格", null);
                throw new Exception("只支持xlsx格式表格");
            }
            try
            {
                mErrorCallback = exportSettings.ErrorCallback;
                mProgressCallback = exportSettings.ProgressCallback;
                ExportTableUtil.Init(exportSettings.TablesPath, exportSettings.KeywordSheetName, exportSettings.ConfigTableName);
                DataTablesExporter dataExporter = new DataTablesExporter(exportSettings.ConfigTableName, exportSettings.TableSheetName, exportSettings.TablesPath);
                TextTablesExporter textExporter = new TextTablesExporter(exportSettings.ConfigTableName, exportSettings.TextSheetName, exportSettings.TablesPath);

                dataExporter.ExportDataTable();
                textExporter.ExportTextTable();

                Task.WaitAll(mTaskList.ToArray());
                textExporter.ExportCSCode();
                PacketEditor.BuildAll(exportSettings.EncryPath, exportSettings.NoEncryPath);
            }
            catch (Exception e)
            {
                mErrorCallback.Invoke($"文件{exportSettings.TablesPath}读取失败:\n{e.Message}", e);
                throw e;
            }
            finally
            {
                mTaskList.Clear();
            }
            exportSettings.CompleteCallback.Invoke();
        });
    }

    internal static void RegisterExportTask(Action value, string key)
    {
        throw new NotImplementedException();
    }
}
