using System.Collections;
using UnityEngine;

public class uWaitForSeconds : IEnumerator
{
    private float _overTime;
    private float _seconds;
    public uWaitForSeconds(float seconds)
    {
        _seconds = seconds;
        _overTime = TimeUtility.GameTime + seconds;
    }

    public object Current
    {
        get
        {
            return null;
        }
    }

    public bool MoveNext()
    {
        if (_overTime > TimeUtility.GameTime)
            return true;

        return false;
    }

    public void Reset()
    {
        _overTime = TimeUtility.GameTime + _seconds;
    }
}
