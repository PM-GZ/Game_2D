using System.Collections;

public class uWaitForSeconds : IEnumerator
{
    private float _overTime;
    private float _seconds;
    public uWaitForSeconds(float seconds)
    {
        _seconds = seconds;
        _overTime = Global.GameTime + seconds;
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
        if (_overTime > Global.GameTime)
            return true;

        return false;
    }

    public void Reset()
    {
        _overTime = Global.GameTime + _seconds;
    }
}
