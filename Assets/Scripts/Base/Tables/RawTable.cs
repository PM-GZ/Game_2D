using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;


public static class TABLE
{
    static Dictionary<Type, TableData> mTables = new Dictionary<Type, TableData>();

    public static T Get<T>() where T : TableData, new()
    {
        return (T)Get(typeof(T));
    }

    public static void UnloadAll()
    {
        mTables.Clear();
    }

    public static TableData Get(Type type)
    {
        TableData t;
        if (mTables.TryGetValue(type, out t))
        {
            return t;
        }
        else
        {
            t = (TableData)Activator.CreateInstance(type);
            mTables.Add(type, t);
            return t;
        }
    }

    public static void Load(params Type[] preloadTables)
    {
        foreach (var v in preloadTables)
        {
            mTables[v] = Activator.CreateInstance(v) as TableData;
        }
    }

    public static void LoadEx(params Type[] preloadTables)
    {
        foreach (var v in preloadTables)
        {
            if (!mTables.ContainsKey(v))
            {
                mTables.Add(v, Activator.CreateInstance(v) as TableData);
            }
        }
    }
}

public class TableData
{
    public string PACKET_NAME { get; protected set; }
    protected RawTable mTableData;
}


public class RawTable
{
    public object[,] _data;
    public string[] _colType;
    public int _nRows;
    public int _nColumns;

    public void ReadBinary(string tableName, long position, string packetName)
    {
        ClearData();
        var bytes = PacketUtils.GetPacket(packetName);
        if (bytes == null) return;

        MemoryStream f = new MemoryStream(bytes);
        f.Position = position;

        BinaryReader br = new BinaryReader(f, Encoding.UTF8);
        int rows = br.ReadInt32();
        int columns = br.ReadInt32();

        _nRows = rows;
        _nColumns = columns;

        if (_nRows == 0 || _nColumns == 0)
        {
            throw new Exception("Error reading tablesize Rows is " + _nRows.ToString() + " and _nColumns is " + _nColumns.ToString() + ".");
        }

        _colType = new string[_nColumns];
        for (int j = 0; j < columns; j++)
        {
            _colType[j] = br.ReadString();
        }

        _data = new object[_nColumns, _nRows];
        for (int i = 0; i < columns; i++)
        {
            string type = _colType[i];

            if (type == "string")
            {
                for (int j = 0; j < rows; j++)
                {
                    _data[i, j] = br.ReadString();
                }
            }
            else if (type == "byte")
            {
                for (int j = 0; j < rows; j++)
                {
                    _data[i, j] = br.ReadByte();
                }
            }
            else if (type == "sbyte")
            {
                for (int j = 0; j < rows; j++)
                {
                    _data[i, j] = br.ReadSByte();
                }
            }
            else if (type == "short")
            {
                for (int j = 0; j < rows; j++)
                {
                    _data[i, j] = br.ReadInt16();
                }
            }
            else if (type == "ushort")
            {
                for (int j = 0; j < rows; j++)
                {
                    _data[i, j] = br.ReadUInt16();
                }
            }
            else if (type == "int")
            {
                for (int j = 0; j < rows; j++)
                {
                    _data[i, j] = br.ReadInt32();
                }
            }
            else if (type == "uint")
            {
                for (int j = 0; j < rows; j++)
                {
                    _data[i, j] = br.ReadUInt32();
                }
            }
            else if (type == "long")
            {
                for (int j = 0; j < rows; j++)
                {
                    _data[i, j] = br.ReadInt64();
                }
            }
            else if (type == "ulong")
            {
                for (int j = 0; j < rows; j++)
                {
                    _data[i, j] = br.ReadInt64();
                }
            }
            else if (type == "float")
            {
                for (int j = 0; j < rows; j++)
                {
                    _data[i, j] = br.ReadSingle();
                }
            }
            else if (type == "double")
            {
                for (int j = 0; j < rows; j++)
                {
                    _data[i, j] = br.ReadDouble();
                }
            }
            else if (type == "bool")
            {
                for (int j = 0; j < rows; j++)
                {
                    _data[i, j] = br.ReadSByte() != 0;
                }
            }
            else if (type == "enum" || type.StartsWith("enum:"))
            {
                for (int j = 0; j < rows; j++)
                {
                    _data[i, j] = br.ReadInt32();
                }
            }
            else if (type == "DateTime")
            {
                for (var j = 0; j < rows; j++)
                {
                    if (!DateTime.TryParseExact(br.ReadString(), "yyyy-MM-dd HH:mm:ss",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
                    {
                        throw new Exception("Invalid DateTime data!");
                    }

                    _data[i, j] = dateTime;
                }
            }
            else if (type == "int[]")
            {
                for (int j = 0; j < rows; j++)
                {
                    var array = new int[br.ReadInt32()];
                    for (var k = 0; k < array.Length; k++) array[k] = br.ReadInt32();
                    _data[i, j] = array;
                }
            }
            else if (type == "uint[]")
            {
                for (int j = 0; j < rows; j++)
                {
                    var array = new uint[br.ReadInt32()];
                    for (var k = 0; k < array.Length; k++) array[k] = br.ReadUInt32();
                    _data[i, j] = array;
                }
            }
            else if (type == "float[]")
            {
                for (int j = 0; j < rows; j++)
                {
                    var array = new float[br.ReadInt32()];
                    for (var k = 0; k < array.Length; k++) array[k] = br.ReadSingle();
                    _data[i, j] = array;
                }
            }
            else if (type == "double[]")
            {
                for (int j = 0; j < rows; j++)
                {
                    var array = new double[br.ReadInt32()];
                    for (var k = 0; k < array.Length; k++) array[k] = br.ReadDouble();
                    _data[i, j] = array;
                }
            }
            else if (type == "string[]")
            {
                for (int j = 0; j < rows; j++)
                {
                    var array = new string[br.ReadInt32()];
                    for (var k = 0; k < array.Length; k++) array[k] = br.ReadString();
                    _data[i, j] = array;
                }
            }
            else if (type == "int[][]")
            {
                for (int j = 0; j < rows; j++)
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
                    _data[i, j] = array;
                }
            }
            else if (type == "uint[][]")
            {
                for (int j = 0; j < rows; j++)
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
                    _data[i, j] = array;
                }
            }
            else if (type == "float[][]")
            {
                for (int j = 0; j < rows; j++)
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
                    _data[i, j] = array;
                }
            }
            else if (type == "double[][]")
            {
                for (int j = 0; j < rows; j++)
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
                    _data[i, j] = array;
                }
            }
            else if (type == "string[][]")
            {
                for (int j = 0; j < rows; j++)
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
                    _data[i, j] = array;
                }
            }
            //else if (type == "xVector3")
            //{
            //    for (int j = 0; j < rows; j++)
            //    {
            //        xVector3 data = xVector3.Zero;
            //        string str = br.ReadString();
            //        string[] items = str.Split(',');
            //        try
            //        {
            //            data.x = float.Parse(items[0]);
            //            data.y = float.Parse(items[1]);
            //            data.z = float.Parse(items[2]);
            //        }
            //        catch
            //        {
            //            throw new Exception("Wrong row or column !" + " Row: " + i + " Columns: " + j + "¡¡data = " + str);
            //        }
            //        _data[i, j] = data;
            //    }
            //}
            else
            {
                throw new Exception("Unrecognized type! type = " + type);
            }
        }
        f.Close();
        br.Close();
    }

    public string GetString(int row, int column)
    {
        if (!CheckRowCol(row, column)) return string.Empty;

        if (_colType[column] == "string")
        {
            return (string)_data[column, row];
        }
        else
        {
            throw new Exception("Wrong Type Set!" + row.ToString() + " Columns: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }
    public byte GetByte(int row, int column)
    {
        if (!CheckRowCol(row, column)) return 0;
        if (_colType[column] == "byte")
        {
            return (byte)_data[column, row];
        }
        else
        {
            throw new Exception("Error Format set: " + row.ToString() + " Columns: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }
    public sbyte GetSByte(int row, int column)
    {
        if (!CheckRowCol(row, column)) return 0;
        if (_colType[column] == "sbyte")
        {
            return (sbyte)_data[column, row];
        }
        else
        {
            throw new Exception("Error Format set: " + row.ToString() + " Columns: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }
    public int GetInt(int row, int column)
    {
        if (!CheckRowCol(row, column)) return 0;

        if (_colType[column] == "int" || _colType[column] == "Int32")
        {
            return Convert.ToInt32(_data[column, row]);
        }
        else
        {
            throw new Exception("Error Format set Row : " + row.ToString() + " Column: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }

    public int[] GetIntArray(int row, int column)
    {
        if (!CheckRowCol(row, column)) return null;
        if (_colType[column] == "int[]")
        {
            return (int[])_data[column, row];
        }
        else
        {
            throw new Exception("Error Format set Row : " + row.ToString() + " Column: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }
    public int[][] GetIntArray2(int row, int column)
    {
        if (!CheckRowCol(row, column)) return null;
        if (_colType[column] == "int[][]")
        {
            return (int[][])_data[column, row];
        }
        else
        {
            throw new Exception("Error Format set Row : " + row.ToString() + " Column: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }
    public int GetEnum(int row, int column)
    {
        if (!CheckRowCol(row, column)) return 0;

        if (_colType[column] == "enum" || _colType[column].StartsWith("enum"))
        {
            return Convert.ToInt32(_data[column, row]);
        }
        else
        {
            throw new Exception("Error Format set Row : " + row.ToString() + " Column: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }
    public uint GetUInt(int row, int column)
    {
        if (!CheckRowCol(row, column)) return 0;

        if (_colType[column] == "uint" || _colType[column] == "UInt32")
        {
            return Convert.ToUInt32(_data[column, row]);
        }
        else
        {
            throw new Exception("Error Format set Row : " + row.ToString() + " Column: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }
    public uint[] GetUIntArray(int row, int column)
    {
        if (!CheckRowCol(row, column)) return null;
        if (_colType[column] == "uint[]")
        {
            return (uint[])_data[column, row];
        }
        else
        {
            throw new Exception("Error Format set Row : " + row.ToString() + " Column: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }
    public uint[][] GetUIntArray2(int row, int column)
    {
        if (!CheckRowCol(row, column)) return null;
        if (_colType[column] == "uint[][]")
        {
            return (uint[][])_data[column, row];
        }
        else
        {
            throw new Exception("Error Format set Row : " + row.ToString() + " Column: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }
    public short GetShort(int row, int column)
    {
        if (!CheckRowCol(row, column)) return 0;

        if (_colType[column] == "short" || _colType[column] == "Int16")
        {
            return Convert.ToInt16(_data[column, row]);
        }
        else
        {
            throw new Exception("Error Format set: " + row.ToString() + " Columns: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }

    public ushort GetUShort(int row, int column)
    {
        if (!CheckRowCol(row, column)) return 0;

        if (_colType[column] == "ushort" || _colType[column] == "UInt16")
        {
            return Convert.ToUInt16(_data[column, row]);
        }
        else
        {
            throw new Exception("Error Format set: " + row.ToString() + " Columns: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }
    public long GetLong(int row, int column)
    {
        if (!CheckRowCol(row, column)) return 0;

        if (_colType[column] == "long" || _colType[column] == "Int64")
        {
            return Convert.ToInt64(_data[column, row]);
        }
        else
        {
            throw new Exception("Error Format set: " + row.ToString() + " Columns: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }
    public ulong GetULong(int row, int column)
    {
        if (!CheckRowCol(row, column)) return 0;

        if (_colType[column] == "ulong" || _colType[column] == "UInt64")
        {
            return Convert.ToUInt64(_data[column, row]);
        }
        else
        {
            throw new Exception("Error Format set: " + row.ToString() + " Columns: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }
    public float GetFloat(int row, int column)
    {
        if (!CheckRowCol(row, column)) return 0;

        if (_colType[column] == "float")
        {
            return (float)_data[column, row];
        }
        else
        {
            throw new Exception("Error Format set: " + row.ToString() + " Columns: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }
    public float[] GetFloatArray(int row, int column)
    {
        if (!CheckRowCol(row, column)) return null;
        if (_colType[column] == "float[]")
        {
            return (float[])_data[column, row];
        }
        else
        {
            throw new Exception("Error Format set Row : " + row.ToString() + " Column: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }
    public float[][] GetFloatArray2(int row, int column)
    {
        if (!CheckRowCol(row, column)) return null;
        if (_colType[column] == "float[][]")
        {
            return (float[][])_data[column, row];
        }
        else
        {
            throw new Exception("Error Format set Row : " + row.ToString() + " Column: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }
    public double GetDouble(int row, int column)
    {
        if (!CheckRowCol(row, column)) return 0;

        if (_colType[column] == "double")
        {
            return (double)_data[column, row];
        }
        else
        {
            throw new Exception("Error Format set: " + row.ToString() + " Columns: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }
    public double[] GetDoubleArray(int row, int column)
    {
        if (!CheckRowCol(row, column)) return null;
        if (_colType[column] == "double[]")
        {
            return (double[])_data[column, row];
        }
        else
        {
            throw new Exception("Error Format set Row : " + row.ToString() + " Column: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }
    public string[] GetStringArray(int row, int column)
    {
        if (!CheckRowCol(row, column)) return null;
        if (_colType[column] == "string[]")
        {
            return (string[])_data[column, row];
        }
        else
        {
            throw new Exception("Error Format set Row : " + row.ToString() + " Column: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }
    public double[][] GetDoubleArray2(int row, int column)
    {
        if (!CheckRowCol(row, column)) return null;
        if (_colType[column] == "double[][]")
        {
            return (double[][])_data[column, row];
        }
        else
        {
            throw new Exception("Error Format set Row : " + row.ToString() + " Column: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }
    public string[][] GetStringArray2(int row, int column)
    {
        if (!CheckRowCol(row, column)) return null;
        if (_colType[column] == "string[][]")
        {
            return (string[][])_data[column, row];
        }
        else
        {
            throw new Exception("Error Format set Row : " + row.ToString() + " Column: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }
    public bool GetBool(int row, int column)
    {
        if (!CheckRowCol(row, column)) return false;

        if (_colType[column] == "bool")
        {
            return (bool)_data[column, row];
        }
        else
        {
            throw new Exception("Error Format set: " + row.ToString() + " Columns: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }
    //public xVector3 GetVector3(int row, int column)
    //{
    //    if (!CheckRowCol(row, column)) return xVector3.Zero;

    //    if (_colType[column] == "Vector3")
    //    {
    //        return (xVector3)_data[column, row];
    //    }
    //    else
    //    {
    //        throw new Exception("Error Format set: " + row.ToString() + " Columns: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
    //    }
    //}

    public DateTime GetDateTime(int row, int column)
    {
        if (!CheckRowCol(row, column))
        {
            return DateTime.Now;
        }

        if (_colType[column] == "DateTime")
        {
            return (DateTime)_data[column, row];
        }
        else
        {
            throw new Exception("Error Format set: " + row.ToString() + " Columns: " + column.ToString() + ". Excel Type: " + _colType[column] + " . Code read Type : " + _data[column, row].GetType());
        }
    }

    void ClearData()
    {
        _data = null;
    }
    bool CheckRowCol(int row, int col)
    {
        if (row < 0 || row >= _nRows || col < 0 || col >= _nColumns)
        {
            throw new Exception("Wrong row or column !" + " Row: " + row.ToString() + " Columns: " + col.ToString());
        }
        return true;
    }
}
