using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;


public class GeneratorAssetNameScript
{
    private const string OUT_PATH = "Assets/Scripts/Game/Common/AssetsName.cs";

    private static StringBuilder mBuilder = new StringBuilder();


    [MenuItem("Tools/生成资源名脚本")]
    private static void CreateAssetNameScript()
    {
        mBuilder.Clear();
        List<AddressableAssetEntry> entries = new();

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        settings.GetAllAssets(entries, true, OnGroupFilter, OnEntryFilter);

        mBuilder.Append($"public static class AssetsName\n");
        mBuilder.Append($"{{\n");
        foreach (AddressableAssetEntry entry in entries)
        {
            string[] strs = entry.address.Split('/');
            string fieldName = strs[strs.Length - 1];
            fieldName = fieldName.Replace('.', '_');
            mBuilder.AppendLine($"\tpublic const string {fieldName} = \"{entry.address}\";");
        }
        mBuilder.Append($"}}");

        File.WriteAllText(OUT_PATH, mBuilder.ToString());
        AssetDatabase.Refresh();
    }

    private static bool OnGroupFilter(AddressableAssetGroup group)
    {
        if (group.name.Equals("Built In Data"))
            return false;

        return true;
    }

    private static bool OnEntryFilter(AddressableAssetEntry entry)
    {
        if (entry.AssetPath.StartsWith("Assets/GameAssets"))
            return true;

        return false;
    }
}
