


public class uGameData : BaseData
{
    public GameLevelDesSO gameLevelDesSO { get; private set; }


    public override void InitData()
    {
        InitConfigs();
    }

    public override void ClearData()
    {
        gameLevelDesSO = null;
    }


    #region º”‘ÿ≈‰÷√Œƒº˛
    private void InitConfigs()
    {
        LoadGameLevelCfg();
    }

    private void LoadGameLevelCfg()
    {
        gameLevelDesSO = Main.Asset.LoadAsset<GameLevelDesSO>("GameLevelDesCfg");
    }
    #endregion
}
