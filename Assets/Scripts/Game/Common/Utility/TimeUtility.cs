using System;
using UnityEngine;

public class TimeUtility
{
    public static float GameTime { get => Time.fixedUnscaledTime; }

    public static ulong GetNowTimeSeconds()
    {
        DateTime dt1970 = new DateTime(1970, 1, 1);
        TimeSpan span = DateTime.Now - dt1970;
        return (ulong)span.TotalSeconds;
    }
}
