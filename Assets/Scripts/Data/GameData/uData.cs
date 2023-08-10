




public class uData : BaseObject
{
    public uGameData GameData { get; private set; }
    public uMapData Map { get; private set; }
    public uPlayerData Player { get; private set; }

    public uData()
    {
        InitDataClass();
        InitEvent();
        InitData();
    }

    private void InitDataClass()
    {
        Map = new();
        Player = new();
        GameData = new();
    }

    private void InitEvent()
    {
        Map.InitEvent();
        Player.InitEvent();
    }
    private void InitData()
    { 
        Map.InitData();
        Player.InitData();
        GameData.InitData();
    }

    private void ClearEvent()
    { 
        Map.ClearEvent();
        Player.ClearEvent();
    }
    private void ClearData()
    { 
        Map.ClearData();
        Player.ClearData();
        GameData.ClearData();
    }

    public void OnDestroy()
    {
        ClearEvent();
        ClearData();
    }
}
