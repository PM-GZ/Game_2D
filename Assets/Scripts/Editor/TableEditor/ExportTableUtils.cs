using System;
using System.Collections.Generic;
using System.IO;

public static class ExportTableUtils
{
    public const string PacketPath = "Builtin/";


    public static Dictionary<string, Type> enumTypes = new Dictionary<string, Type>();


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
}
