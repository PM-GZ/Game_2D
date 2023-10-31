using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;



public class Global
{
    public static float GameTime { get => Time.time; }

    static Dictionary<string, uint> mStr2HashCached = new Dictionary<string, uint>();
    static Dictionary<uint, string> mConvertedHash = new Dictionary<uint, string>();

    public static uint GetStr2Hash(string value)
    {
        if (string.IsNullOrEmpty(value)) return 0;
        if (mStr2HashCached.TryGetValue(value, out var hash))
        {
            return hash;
        }
        const uint seed = 0xc58f1a7a;
        const uint m = 0x5bd1e995;
        const int r = 24;
        var data = Encoding.UTF8.GetBytes(value);
        int length = data.Length;
        if (length == 0)
        {
            mStr2HashCached.Add(value, 0);
            mConvertedHash.Add(0, value);
            return 0;
        }
        uint h = seed ^ (uint)length;
        int currentIndex = 0;
        while (length >= 4)
        {
            uint k = (uint)(data[currentIndex++] | data[currentIndex++] << 8 | data[currentIndex++] << 16 | data[currentIndex++] << 24);
            k *= m;
            k ^= k >> r;
            k *= m;
            h *= m;
            h ^= k;
            length -= 4;
        }
        switch (length)
        {
            case 3:
                h ^= (UInt16)(data[currentIndex++] | data[currentIndex++] << 8);
                h ^= (uint)(data[currentIndex] << 16);
                h *= m;
                break;
            case 2:
                h ^= (UInt16)(data[currentIndex++] | data[currentIndex] << 8);
                h *= m;
                break;
            case 1:
                h ^= data[currentIndex];
                h *= m;
                break;
            default:
                break;
        }

        h ^= h >> 13;
        h *= m;
        h ^= h >> 15;

        mStr2HashCached.Add(value, h);
        mConvertedHash.Add(h, value);
        return h;
    }
}
