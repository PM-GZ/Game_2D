




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

    private void InitEvent()
    {
        Player.InitEvent();
    }
    private void InitData()
    { 
        Player.InitData();
        GameData.InitData();
    }

    private void ClearEvent()
    { 
        Player.ClearEvent();
    }
    private void ClearData()
    { 
        Player.ClearData();
        GameData.ClearData();
    }

    public void OnDestroy()
    {
        Player.SavePlayerData();
        ClearEvent();
        ClearData();
    }
}
