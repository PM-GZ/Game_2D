using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;



public class QuickCSharp
{
    private static StringBuilder _sBuild = new();
    private static int _count = 1;

    [MenuItem("C# Script", menuItem = "Assets/Create/Quick C# #&C")]
    private static void QuickCreateCSharp()
    {
        string floder = GetSelectedFloder();
        string path = GetPath(floder);
        CreateCSharp(Path.GetFileNameWithoutExtension(path));
        File.WriteAllText(path, _sBuild.ToString());
        AssetDatabase.Refresh();
    }

    private static string GetSelectedFloder()
    {
        var filters = Selection.GetFiltered<Object>(SelectionMode.Assets);
        foreach (var filter in filters)
        {
            string path = AssetDatabase.GetAssetPath(filter);
            if (string.IsNullOrEmpty(path)) continue;

            if (Directory.Exists(path))
                return path;

            if (File.Exists(path))
                return Path.GetDirectoryName(path);
        }
        return "Assets";
    }

    private static string GetPath(string floder)
    {
        string path = $"{floder}/CSharpScript.cs";
        if (File.Exists(path))
        {
            path = $"{floder}/CSharpScript{_count}.cs";
            _count++;
        }
        else
        {
            _count = 1;
        }
        return path;
    }

    private static void CreateCSharp(string className)
    {
        _sBuild.Clear();

        _sBuild.Append("using UnityEngine;");
        _sBuild.Append($"\n\n\n\npublic class {className} : MonoBehaviour");
        _sBuild.Append("\n{");

        _sBuild.Append("\n\tprivate void Start()");
        _sBuild.Append("\n\t{");
        _sBuild.Append("\n\t}");

        _sBuild.Append("\n\tprivate void Update()");
        _sBuild.Append("\n\t{");
        _sBuild.Append("\n\t}");

        _sBuild.Append("\n}");
    }
}
