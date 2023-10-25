using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

public static class TABLE
{
    private static Dictionary<Type, TableData> _TableDataDict = new();

    public static void SetTableData(Type type, TableData data)
    {
        _TableDataDict.Add(type, data);
    }

    public static T Get<T>() where T : TableData
    {
        Type type = typeof(T);
        if (!_TableDataDict.ContainsKey(type))
        {
            T t = Activator.CreateInstance<T>();
            _TableDataDict.Add(type, t);
        }
        return _TableDataDict[type] as T;
    }
}

public class TableData
{
    protected RawTable rawTable;
}

public class RawTable
{
    public object[,] data;
    public string[] colType;
    public int rowNum;
    public int colNum;

    public void ReadTable(string tableName, string sheetName)
    {
        data = null;

        using ExcelWorksheet sheet = TableUtility.GetTable(tableName, sheetName);
        if (sheet == null)
        {
            throw new Exception($"Error sheet {sheetName} int the table£º{tableName} is null");
        }
        TableUtility.RemoveEmptyRow(sheet);

        rowNum = sheet.Dimension.Rows;
        colNum = sheet.Dimension.Columns;
        data = new object[colNum, rowNum - 3];
        colType = new string[colNum];

        for (int i = 1; i <= colNum; i++)
        {
            string type = sheet.GetValue(3, i).ToString();
            colType[i - 1] = type;

            if (type == "string")
            {
                GetValue<string>(rowNum, i, sheet);
            }
            else if (type == "byte")
            {
                GetValue<byte>(rowNum, i, sheet);
            }
            else if (type == "sbyte")
            {
                GetValue<sbyte>(rowNum, i, sheet);
            }
            else if (type == "short")
            {
                GetValue<short>(rowNum, i, sheet);
            }
            else if (type == "ushort")
            {
                GetValue<ushort>(rowNum, i, sheet);
            }
            else if (type == "int")
            {
                GetValue<int>(rowNum, i, sheet);
            }
            else if (type == "uint")
            {
                GetValue<uint>(rowNum, i, sheet);
            }
            else if (type == "long")
            {
                GetValue<long>(rowNum, i, sheet);
            }
            else if (type == "ulong")
            {
                GetValue<ulong>(rowNum, i, sheet);
            }
            else if (type == "float")
            {
                GetValue<float>(rowNum, i, sheet);
            }
            else if (type == "double")
            {
                GetValue<double>(rowNum, i, sheet);
            }
            else if (type == "bool")
            {
                GetValue<bool>(rowNum, i, sheet);
            }
            else if (type == "enum" || type.StartsWith("enum:"))
            {
                for (int j = 4; j <= rowNum; j++)
                {
                    data[i - 1, j - 4] = sheet.GetValue(j, i);
                }
            }
            else if (type == "DateTime")
            {
                for (var j = 4; j <= rowNum; j++)
                {
                    if (!DateTime.TryParseExact(sheet.GetValue(j, i).ToString(), "yyyy-MM-dd HH:mm:ss",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
                    {
                        throw new Exception("Invalid DateTime data!");
                    }

                    data[i - 1, j - 4] = dateTime;
                }
            }
            else if (type == "int[]")
            {
                for (int j = 4; j <= rowNum; j++)
                {
                    string value = sheet.GetValue(j, i).ToString();
                    var arr = value.Split(',');
                    var array = new int[arr.Length];
                    for (var k = 0; k < array.Length; k++) array[k] = int.Parse(arr[k]);
                    data[i - 1, j - 4] = array;
                }
            }
            else if (type == "uint[]")
            {
                for (int j = 4; j <= rowNum; j++)
                {
                    string value = sheet.GetValue(j, i).ToString();
                    var arr = value.Split(',');
                    var array = new uint[arr.Length];
                    for (var k = 0; k < array.Length; k++) array[k] = uint.Parse(arr[k]);
                    data[i - 1, j - 4] = array;
                }
            }
            else if (type == "float[]")
            {
                for (int j = 4; j <= rowNum; j++)
                {
                    string value = sheet.GetValue(j, i).ToString();
                    var arr = value.Split(',');
                    var array = new float[arr.Length];
                    for (var k = 0; k < array.Length; k++) array[k] = float.Parse(arr[k]);
                    data[i - 1, j - 4] = array;
                }
            }
            else if (type == "double[]")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    string value = sheet.GetValue(j, i).ToString();
                    var arr = value.Split(',');
                    var array = new double[arr.Length];
                    for (var k = 0; k < array.Length; k++) array[k] = double.Parse(arr[k]);
                    data[i - 1, j - 4] = array;
                }
            }
            else if (type == "string[]")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    string value = sheet.GetValue(j, i).ToString();
                    var arr = value.Split(',');
                    var array = new string[arr.Length];
                    for (var k = 0; k < array.Length; k++) array[k] = arr[k];
                    data[i - 1, j - 4] = array;
                }
            }
            else if (type == "int[][]")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    string value = sheet.GetValue(j, i).ToString();
                    var arr = value.Split('|');

                    var array = new int[arr.Length][];
                    for (var k = 0; k < array.Length; k++)
                    {
                        var arr2 = arr[k].Split(',');

                        array[k] = new int[arr2.Length];
                        for (int m = 0; m < arr2.Length; m++)
                        {
                            array[k][m] = int.Parse(arr2[m]);
                        }
                    }
                    data[i, j] = array;
                }
            }
            else if (type == "uint[][]")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    string value = sheet.GetValue(j, i).ToString();
                    var arr = value.Split('|');

                    var array = new uint[arr.Length][];
                    for (var k = 0; k < array.Length; k++)
                    {
                        var arr2 = arr[k].Split(',');

                        array[k] = new uint[arr2.Length];
                        for (int m = 0; m < arr2.Length; m++)
                        {
                            array[k][m] = uint.Parse(arr2[m]);
                        }
                    }
                    data[i, j] = array;
                }
            }
            else if (type == "float[][]")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    string value = sheet.GetValue(j, i).ToString();
                    var arr = value.Split('|');

                    var array = new float[arr.Length][];
                    for (var k = 0; k < array.Length; k++)
                    {
                        var arr2 = arr[k].Split(',');

                        array[k] = new float[arr2.Length];
                        for (int m = 0; m < arr2.Length; m++)
                        {
                            array[k][m] = float.Parse(arr2[m]);
                        }
                    }
                    data[i, j] = array;
                }
            }
            else if (type == "double[][]")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    string value = sheet.GetValue(j, i).ToString();
                    var arr = value.Split('|');

                    var array = new double[arr.Length][];
                    for (var k = 0; k < array.Length; k++)
                    {
                        var arr2 = arr[k].Split(',');

                        array[k] = new double[arr2.Length];
                        for (int m = 0; m < arr2.Length; m++)
                        {
                            array[k][m] = double.Parse(arr2[m]);
                        }
                    }
                    data[i, j] = array;
                }
            }
            else if (type == "string[][]")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    string value = sheet.GetValue(j, i).ToString();
                    var arr = value.Split('|');

                    var array = new string[arr.Length][];
                    for (var k = 0; k < array.Length; k++)
                    {
                        var arr2 = arr[k].Split(',');

                        array[k] = new string[arr2.Length];
                        for (int m = 0; m < arr2.Length; m++)
                        {
                            array[k][m] = arr2[m];
                        }
                    }
                    data[i, j] = array;
                }
            }
            else
            {
                throw new Exception("Unrecognized type! type = " + type);
            }
        }
    }

    public void ReadTable(string tableName)
    {
        data = null;

        byte[] file = TableUtility.GetFile(tableName);
        MemoryStream ms = new MemoryStream(file);
        if (ms == null) return;

        BinaryReader br = new BinaryReader(ms, Encoding.UTF8);
        rowNum = br.ReadInt32();
        colNum = br.ReadInt32();
        if (rowNum == 0 || colNum == 0)
        {
            throw new Exception("Error reading tablesize _rowNum is " + rowNum + " and _n_colNum is " + colNum + ".");
        }

        colType = new string[colNum];
        for (int j = 0; j < colNum; j++)
        {
            colType[j] = br.ReadString();
        }

        data = new object[colNum, rowNum];
        for (int i = 0; i < colNum; i++)
        {
            string type = colType[i];

            if (type == "string")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    data[i, j] = br.ReadString();
                }
            }
            else if (type == "byte")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    data[i, j] = br.ReadByte();
                }
            }
            else if (type == "sbyte")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    data[i, j] = br.ReadSByte();
                }
            }
            else if (type == "short")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    data[i, j] = br.ReadInt16();
                }
            }
            else if (type == "ushort")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    data[i, j] = br.ReadUInt16();
                }
            }
            else if (type == "int")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    data[i, j] = br.ReadInt32();
                }
            }
            else if (type == "uint")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    data[i, j] = br.ReadUInt32();
                }
            }
            else if (type == "long")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    data[i, j] = br.ReadInt64();
                }
            }
            else if (type == "ulong")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    data[i, j] = br.ReadInt64();
                }
            }
            else if (type == "float")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    data[i, j] = br.ReadSingle();
                }
            }
            else if (type == "double")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    data[i, j] = br.ReadDouble();
                }
            }
            else if (type == "bool")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    data[i, j] = br.ReadSByte() != 0;
                }
            }
            else if (type == "enum" || type.StartsWith("enum:"))
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    data[i, j] = br.ReadInt32();
                }
            }
            else if (type == "DateTime")
            {
                for (var j = 0; j <= rowNum; j++)
                {
                    if (!DateTime.TryParseExact(br.ReadString(), "yyyy-MM-dd HH:mm:ss",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
                    {
                        throw new Exception("Invalid DateTime data!");
                    }

                    data[i, j] = dateTime;
                }
            }
            else if (type == "int[]")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    var array = new int[br.ReadInt32()];
                    for (var k = 0; k < array.Length; k++) array[k] = br.ReadInt32();
                    data[i, j] = array;
                }
            }
            else if (type == "uint[]")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    var array = new uint[br.ReadInt32()];
                    for (var k = 0; k < array.Length; k++) array[k] = br.ReadUInt32();
                    data[i, j] = array;
                }
            }
            else if (type == "float[]")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    var array = new float[br.ReadInt32()];
                    for (var k = 0; k < array.Length; k++) array[k] = br.ReadSingle();
                    data[i, j] = array;
                }
            }
            else if (type == "double[]")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    var array = new double[br.ReadInt32()];
                    for (var k = 0; k < array.Length; k++) array[k] = br.ReadDouble();
                    data[i, j] = array;
                }
            }
            else if (type == "string[]")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    var array = new string[br.ReadInt32()];
                    for (var k = 0; k < array.Length; k++) array[k] = br.ReadString();
                    data[i, j] = array;
                }
            }
            else if (type == "int[][]")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    var array = new int[br.ReadInt32()][];
                    for (var k = 0; k < array.Length; k++)
                    {
                        int length = br.ReadInt32();

                        array[k] = new int[length];

                        for (int m = 0; m < length; m++)
                        {
                            array[k][m] = br.ReadInt32();
                        }
                    }
                    data[i, j] = array;
                }
            }
            else if (type == "uint[][]")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    var array = new uint[br.ReadInt32()][];
                    for (var k = 0; k < array.Length; k++)
                    {
                        int length = br.ReadInt32();

                        array[k] = new uint[length];

                        for (int m = 0; m < length; m++)
                        {
                            array[k][m] = br.ReadUInt32();
                        }
                    }
                    data[i, j] = array;
                }
            }
            else if (type == "float[][]")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    var array = new float[br.ReadInt32()][];
                    for (var k = 0; k < array.Length; k++)
                    {
                        int length = br.ReadInt32();

                        array[k] = new float[length];

                        for (int m = 0; m < length; m++)
                        {
                            array[k][m] = br.ReadSingle();
                        }
                    }
                    data[i, j] = array;
                }
            }
            else if (type == "double[][]")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    var array = new double[br.ReadInt32()][];
                    for (var k = 0; k < array.Length; k++)
                    {
                        int length = br.ReadInt32();

                        array[k] = new double[length];

                        for (int m = 0; m < length; m++)
                        {
                            array[k][m] = br.ReadDouble();
                        }
                    }
                    data[i, j] = array;
                }
            }
            else if (type == "string[][]")
            {
                for (int j = 0; j <= rowNum; j++)
                {
                    var array = new string[br.ReadInt32()][];
                    for (var k = 0; k < array.Length; k++)
                    {
                        int length = br.ReadInt32();

                        array[k] = new string[length];

                        for (int m = 0; m < length; m++)
                        {
                            array[k][m] = br.ReadString();
                        }
                    }
                    data[i, j] = array;
                }
            }
            else
            {
                throw new Exception("Unrecognized type! type = " + type);
            }
        }
        ms.Close();
        br.Close();
    }

    private void GetValue<T>(int rowNum, int i, ExcelWorksheet sheet)
    {
        for (int j = 4; j <= rowNum; j++)
        {
            data[i - 1, j - 4] = sheet.GetValue<T>(j, i);
        }
    }

    #region GetValue Func
    private Exception ErrorTip(int row, int column)
    {
        return new Exception("Wrong Type Set!" + row.ToString() + " Columns: " + column.ToString() + ". Excel Type: " + colType[column] + " . Code read Type : " + data[column, row].GetType());
    }

    public string GetString(int row, int column)
    {
        if (!CheckRowCol(row, column)) return string.Empty;

        if (colType[column] == "string")
        {
            return (string)data[column, row];
        }
        throw ErrorTip(row, column);
    }
    public byte GetByte(int row, int column)
    {
        if (!CheckRowCol(row, column)) return 0;
        if (colType[column] == "byte")
        {
            return (byte)data[column, row];
        }
        throw ErrorTip(row, column);
    }
    public sbyte GetSByte(int row, int column)
    {
        if (!CheckRowCol(row, column)) return 0;
        if (colType[column] == "sbyte")
        {
            return (sbyte)data[column, row];
        }
        throw ErrorTip(row, column);
    }
    public int GetInt(int row, int column)
    {
        if (!CheckRowCol(row, column)) return 0;

        if (colType[column] == "int" || colType[column] == "Int32")
        {
            return Convert.ToInt32(data[column, row]);
        }
        throw ErrorTip(row, column);
    }

    public int[] GetIntArray(int row, int column)
    {
        if (!CheckRowCol(row, column)) return null;
        if (colType[column] == "int[]")
        {
            return (int[])data[column, row];
        }
        throw ErrorTip(row, column);
    }
    public int[][] GetIntArray2(int row, int column)
    {
        if (!CheckRowCol(row, column)) return null;
        if (colType[column] == "int[][]")
        {
            return (int[][])data[column, row];
        }
        throw ErrorTip(row, column);
    }
    public int GetEnum(int row, int column)
    {
        if (!CheckRowCol(row, column)) return 0;

        if (colType[column] == "enum" || colType[column].StartsWith("enum"))
        {
            return Convert.ToInt32(data[column, row]);
        }
        throw ErrorTip(row, column);
    }
    public uint GetUInt(int row, int column)
    {
        if (!CheckRowCol(row, column)) return 0;

        if (colType[column] == "uint" || colType[column] == "UInt32")
        {
            return Convert.ToUInt32(data[column, row]);
        }
        throw ErrorTip(row, column);
    }
    public uint[] GetUIntArray(int row, int column)
    {
        if (!CheckRowCol(row, column)) return null;
        if (colType[column] == "uint[]")
        {
            return (uint[])data[column, row];
        }
        throw ErrorTip(row, column);
    }
    public uint[][] GetUIntArray2(int row, int column)
    {
        if (!CheckRowCol(row, column)) return null;
        if (colType[column] == "uint[][]")
        {
            return (uint[][])data[column, row];
        }
        throw ErrorTip(row, column);
    }
    public short GetShort(int row, int column)
    {
        if (!CheckRowCol(row, column)) return 0;

        if (colType[column] == "short" || colType[column] == "Int16")
        {
            return Convert.ToInt16(data[column, row]);
        }
        throw ErrorTip(row, column);
    }

    public ushort GetUShort(int row, int column)
    {
        if (!CheckRowCol(row, column)) return 0;

        if (colType[column] == "ushort" || colType[column] == "UInt16")
        {
            return Convert.ToUInt16(data[column, row]);
        }
        throw ErrorTip(row, column);
    }
    public long GetLong(int row, int column)
    {
        if (!CheckRowCol(row, column)) return 0;

        if (colType[column] == "long" || colType[column] == "Int64")
        {
            return Convert.ToInt64(data[column, row]);
        }
        throw ErrorTip(row, column);
    }
    public ulong GetULong(int row, int column)
    {
        if (!CheckRowCol(row, column)) return 0;

        if (colType[column] == "ulong" || colType[column] == "UInt64")
        {
            return Convert.ToUInt64(data[column, row]);
        }
        throw ErrorTip(row, column);
    }
    public float GetFloat(int row, int column)
    {
        if (!CheckRowCol(row, column)) return 0;

        if (colType[column] == "float")
        {
            return (float)data[column, row];
        }
        throw ErrorTip(row, column);
    }
    public float[] GetFloatArray(int row, int column)
    {
        if (!CheckRowCol(row, column)) return null;
        if (colType[column] == "float[]")
        {
            return (float[])data[column, row];
        }
        throw ErrorTip(row, column);
    }
    public float[][] GetFloatArray2(int row, int column)
    {
        if (!CheckRowCol(row, column)) return null;
        if (colType[column] == "float[][]")
        {
            return (float[][])data[column, row];
        }
        throw ErrorTip(row, column);
    }
    public double GetDouble(int row, int column)
    {
        if (!CheckRowCol(row, column)) return 0;

        if (colType[column] == "double")
        {
            return (double)data[column, row];
        }
        throw ErrorTip(row, column);
    }
    public double[] GetDoubleArray(int row, int column)
    {
        if (!CheckRowCol(row, column)) return null;
        if (colType[column] == "double[]")
        {
            return (double[])data[column, row];
        }
        throw ErrorTip(row, column);
    }
    public string[] GetStringArray(int row, int column)
    {
        if (!CheckRowCol(row, column)) return null;
        if (colType[column] == "string[]")
        {
            return (string[])data[column, row];
        }
        throw ErrorTip(row, column);
    }
    public double[][] GetDoubleArray2(int row, int column)
    {
        if (!CheckRowCol(row, column)) return null;
        if (colType[column] == "double[][]")
        {
            return (double[][])data[column, row];
        }
        throw ErrorTip(row, column);
    }
    public string[][] GetStringArray2(int row, int column)
    {
        if (!CheckRowCol(row, column)) return null;
        if (colType[column] == "string[][]")
        {
            return (string[][])data[column, row];
        }
        throw ErrorTip(row, column);
    }
    public bool GetBool(int row, int column)
    {
        if (!CheckRowCol(row, column)) return false;

        if (colType[column] == "bool")
        {
            return (bool)data[column, row];
        }
        throw ErrorTip(row, column);
    }

    public DateTime GetDateTime(int row, int column)
    {
        if (!CheckRowCol(row, column))
        {
            return DateTime.Now;
        }

        if (colType[column] == "DateTime")
        {
            return (DateTime)data[column, row];
        }
        throw ErrorTip(row, column);
    }

    bool CheckRowCol(int row, int col)
    {
        if (row < 0 || row >= rowNum || col < 0 || col >= colNum)
        {
            throw new Exception("Wrong row or column !" + " Row: " + row.ToString() + " Columns: " + col.ToString());
        }
        return true;
    }
    #endregion
}