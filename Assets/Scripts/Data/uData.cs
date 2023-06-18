




public class uData : BaseObject
{
    public uGameData GameData { get; private set; }
    public uPlayerData Player { get; private set; }

    public override void Init()
    {
        InitDataClass();
        InitEvent();
        InitData();
    }

    private void InitDataClass()
    {
        Player = new();
        GameData = new();
    }

    public void InitEvent()
    {
        Player.InitEvent();
    }
    public void InitData()
    { 
        Player.InitData();
        GameData.InitData();
    }

    public void ClearEvent()
    { 
        Player.ClearEvent();
    }
    public void ClearData()
    { 
        Player.ClearData();
        GameData.ClearData();
    }

    public void OnDestroy()
    {
        ClearEvent();
        ClearData();
    }
}
