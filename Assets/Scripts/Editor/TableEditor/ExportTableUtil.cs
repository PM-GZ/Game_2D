using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using OfficeOpenXml;

public static class ExportTableUtil
{
    private static int exportedCount;
    private static int maxTablesCount;
    private static string[] mKeywords;

    public const string SERVER_CS_DIR = "ExportServerCs";
    public static readonly string[] TEXT_HEAD = { "CN", "EN", "TCN" };
    public const string TEXT_TABLE_FILE_NAME = "TEXT_Keys.cs";
    public static Dictionary<string, Type> enumTypes = new Dictionary<string, Type>();


    public static void Init(string tablesPath, string keywordSheetName, string configTableName)
    {
        exportedCount = 0;
        maxTablesCount = 0;
        InitKeyword(tablesPath, keywordSheetName, configTableName);
    }

    private static void InitKeyword(string tablesPath, string keywordSheetName, string configTableName)
    {
        var configTables = ReadExcel(tablesPath + "/" + Path.GetFileNameWithoutExtension(configTableName));
        if (configTables == null)
        {
            throw new Exception(tablesPath + "/" + Path.GetFileNameWithoutExtension(configTableName) + "路径为空!");
        }
        try
        {
            ExcelWorksheet sheet = configTables.Workbook.Worksheets[keywordSheetName];
            mKeywords = new string[sheet.Dimension.Rows];
            for (int i = 0; i < sheet.Dimension.Rows; i++)
            {
                mKeywords[i] = GetCell(sheet, i, 0);
            }
        }
        catch
        {
            throw new Exception("不存在关键字页签:" + keywordSheetName);
        }
    }

    public static ExcelPackage ReadExcel(string filePath)
    {
        if (!File.Exists(filePath + ".xlsx"))
        {
            throw new Exception("文件不存在:" + filePath + ".xlsx");
        }
        try
        {
            using (Stream stream = File.Open(filePath + ".xlsx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return new ExcelPackage(stream);
            }
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public static bool CheckHead(string Header, List<string> tableHeaders)
    {
        if (Header.StartsWith("#")) return true;   //被废弃字段
        if (!Regex.IsMatch(Header, "^[a-zA-Z_][a-zA-Z0-9_]*$"))
        {
            return false;
        }
        for (int i = 0; i < mKeywords.Length; i++)
        {
            if (Header == mKeywords[i])
                return false;
        }
        for (int i = 0; i < tableHeaders.Count; i++)
        {
            if (tableHeaders[i] == Header)
            {
                return false;
            }
        }
        return true;
    }

    public static string GetCell(ExcelWorksheet sheet, int row, int column)
    {
        try
        {
            string str = sheet.GetValue<string>(row + 1, column + 1);

            if (string.IsNullOrEmpty(str))
            {
                str = string.Empty;
            }

            return str;
        }
        catch (Exception e)
        {
            throw e;
        }
    }
    public static void WriteIntArray(string str, BinaryWriter bw)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            bw.Write(0);
        }
        else
        {
            var array = str.Split("#");
            bw.Write(array.Length);
            for (var i = 0; i < array.Length; i++)
            {
                if (int.TryParse(array[i], out var val))
                {
                    bw.Write(val);
                }
                else
                {
                    bw.Write(0);
                }
            }
        }
    }

    public static void WriteIntArray2(string str, BinaryWriter bw)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            bw.Write(0);
        }
        else
        {
            var array = str.Split("#");
            bw.Write(array.Length);
            for (var i = 0; i < array.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(array[i]))
                {
                    bw.Write(0);
                }
                else
                {
                    var array2 = array[i].Split(",");
                    bw.Write(array2.Length);
                    for (int m = 0; m < array2.Length; m++)
                    {
                        if (int.TryParse(array2[m], out var val))
                        {
                            bw.Write(val);
                        }
                        else
                        {
                            bw.Write(0);
                        }
                    }
                }
            }
        }
    }

    public static void WriteUintArray(string str, BinaryWriter bw)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            bw.Write(0);
        }
        else
        {
            var array = str.Split("#");
            bw.Write(array.Length);
            for (var i = 0; i < array.Length; i++)
            {
                if (uint.TryParse(array[i], out var val))
                {
                    bw.Write(val);
                }
                else
                {
                    bw.Write(0u);
                }
            }
        }
    }

    public static void WriteUintArray2(string str, BinaryWriter bw)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            bw.Write(0);
        }
        else
        {
            var array = str.Split("#");
            bw.Write(array.Length);
            for (var i = 0; i < array.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(array[i]))
                {
                    bw.Write(0);
                }
                else
                {
                    var array2 = array[i].Split(",");
                    bw.Write(array2.Length);
                    for (int m = 0; m < array2.Length; m++)
                    {
                        if (uint.TryParse(array2[m], out var val))
                        {
                            bw.Write(val);
                        }
                        else
                        {
                            bw.Write(0u);
                        }
                    }
                }
            }
        }
    }

    public static void WriteFloatArray(string str, BinaryWriter bw)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            bw.Write(0);
        }
        else
        {
            var array = str.Split("#");
            bw.Write(array.Length);
            for (var i = 0; i < array.Length; i++)
            {
                if (float.TryParse(array[i], out var val))
                {
                    bw.Write(val);
                }
                else
                {
                    bw.Write(0f);
                }
            }
        }
    }

    public static void WriteFloatArray2(string str, BinaryWriter bw)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            bw.Write(0);
        }
        else
        {
            var array = str.Split("#");
            bw.Write(array.Length);
            for (var i = 0; i < array.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(array[i]))
                {
                    bw.Write(0);
                }
                else
                {
                    var array2 = array[i].Split(",");
                    bw.Write(array2.Length);
                    for (int m = 0; m < array2.Length; m++)
                    {
                        if (float.TryParse(array2[m], out var val))
                        {
                            bw.Write(val);
                        }
                        else
                        {
                            bw.Write(0f);
                        }
                    }
                }
            }
        }
    }

    public static void WriteDoubleArray(string str, BinaryWriter bw)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            bw.Write(0);
        }
        else
        {
            var array = str.Split("#");
            bw.Write(array.Length);
            for (var i = 0; i < array.Length; i++)
            {
                if (uint.TryParse(array[i], out var val))
                {
                    bw.Write(val);
                }
                else
                {
                    bw.Write(0d);
                }
            }
        }
    }

    public static void WriteStringArray(string str, BinaryWriter bw)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            bw.Write(0);
        }
        else
        {
            var array = str.Split("#");
            bw.Write(array.Length);
            for (var i = 0; i < array.Length; i++)
            {
                bw.Write(array[i]);
            }
        }
    }

    public static void WriteDoubleArray2(string str, BinaryWriter bw)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            bw.Write(0);
        }
        else
        {
            var array = str.Split("#");
            bw.Write(array.Length);
            for (var i = 0; i < array.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(array[i]))
                {
                    bw.Write(0);
                }
                else
                {
                    var array2 = array[i].Split(",");
                    bw.Write(array2.Length);
                    for (int m = 0; m < array2.Length; m++)
                    {
                        if (double.TryParse(array2[m], out var val))
                        {
                            bw.Write(val);
                        }
                        else
                        {
                            bw.Write(0d);
                        }
                    }
                }
            }
        }
    }

    public static void WriteStringArray2(string str, BinaryWriter bw)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            bw.Write(0);
        }
        else
        {
            var array = str.Split("#");
            bw.Write(array.Length);
            for (var i = 0; i < array.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(array[i]))
                {
                    bw.Write(0);
                }
                else
                {
                    var array2 = array[i].Split(",");
                    bw.Write(array2.Length);
                    for (int m = 0; m < array2.Length; m++)
                    {
                        bw.Write(array2[i]);
                    }
                }
            }
        }
    }

    public static int GetExportCount()
    {
        return exportedCount;
    }

    public static int GetMaxTableCount()
    {
        return maxTablesCount;
    }

    internal static void MaxTablesCount(int count)
    {
        maxTablesCount += count;
    }
}
