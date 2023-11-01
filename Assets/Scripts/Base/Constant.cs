using UnityEngine;





public static class Constant
{
    static Constant()
    {
        BUILTIN_PATH = "Builtin/";
    }

    public static string BUILTIN_PATH { get; set; }

    public static string AssetConfigPath = "Assets/EditorAssets/AssetsFloderConfig.asset";
    public static string AssetJsonPath = "Assets/GameAssets/Config/AssetsConfig.json";

#if UNITY_EDITOR
    public static string FILE_WRITE_PATH = "FileData";
#else
    public static string FILE_WRITE_PATH = Application.persistentDataPath + "/FileData";
#endif
    public const string INPUT_SYSTEM_PATH = "Input System/Input System";

    public const string TABLE_CONFIG_PATH = "Assets/EditorAssets/Table";
}
