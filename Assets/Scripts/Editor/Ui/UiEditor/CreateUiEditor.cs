using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateUiEditor : EditorWindow
{
    private VisualElement mRoot;
    private TextField mPanelText;
    private Button mCreate;
    private EnumField mPanelLevel;

    private StringBuilder mBuilder = new StringBuilder();
    private string mFloder;


    private void CreateGUI()
    {
        mRoot = rootVisualElement;
        mRoot.style.justifyContent = Justify.SpaceBetween;

        VisualElement top = new VisualElement();
        top.style.flexShrink = 1;
        top.style.flexGrow = 1;
        mRoot.Add(top);

        mCreate = new Button(OnCreateClick);
        mCreate.text = "Create";
        mRoot.Add(mCreate);

        mPanelText = new TextField("Ui名字");
        top.Add(mPanelText);

        mPanelLevel = new EnumField("PanelLavel", PanelLevel.Normal);
        top.Add(mPanelLevel);
    }

    private void OnCreateClick()
    {
        mBuilder.Clear();

        if (CheckPanelName())
        {
            switch ((PanelLevel)mPanelLevel.value)
            {
                case PanelLevel.Background:
                case PanelLevel.Normal:
                case PanelLevel.Fixed:
                case PanelLevel.Top:
                    mFloder = "Panel";
                    break;
                case PanelLevel.Dialog:
                    mFloder = "Dialog";
                    break;
            }

            CreatePrefab();
            CreateScript();
            CreateConfig();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    private bool CheckPanelName()
    {
        string error = string.Empty;
        if (string.IsNullOrEmpty(mPanelText.value))
        {
            error = "名称不能为空！！！";
        }
        else
        {
            string first = mPanelText.value[0].ToString();
            Regex regex = new Regex("^[a-zA-Z]+$");
            if (!regex.Match(first).Success)
            {
                error = "只能以字母开头";
            }
        }

        if (error != string.Empty)
        {
            EditorUtility.DisplayDialog("Error", $"{error}", "OK");
            return false;
        }
        return true;
    }

    private void CreatePrefab()
    {
        string path = $"Assets/GameAssets/Ui/{mFloder}/{mPanelText.value}";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        if (!File.Exists($"{path}/{mPanelText.value}.prefab"))
        {
            var go = new GameObject(mPanelText.value);
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, $"{path}/{mPanelText.value}.prefab");
            DestroyImmediate(go);
            EditorUtility.SetDirty(prefab);
        }
    }

    private void CreateConfig()
    {
        string path = $"Assets/GameAssets/Data/SOData/UiSOData/{mPanelText.value}SO.asset";
        if (!File.Exists(path))
        {
            var config = ScriptableObject.CreateInstance<UiPanelSOConfig>();
            config.PanelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/GameAssets/Ui/{mFloder}/{mPanelText.value}/{mPanelText.value}.prefab");
            config.Level = (PanelLevel)mPanelLevel.value;
            AssetDatabase.CreateAsset(config, path);
            EditorUtility.SetDirty(config);
        }
    }

    private void CreateScript()
    {
        mBuilder.Append($"\n\n\n\n\n\n[PanelBind(\"{mPanelText.value}\")]\n");
        mBuilder.Append($"public class {mPanelText.value} : BasePanel\n");
        mBuilder.Append($"{{\n");

        mBuilder.Append($"\tpublic override void OnStart()\n");
        mBuilder.Append($"\t{{\n\n");
        mBuilder.Append($"\t\tbase.OnStart();");
        mBuilder.Append($"\t}}\n");

        mBuilder.Append($"}}\n");

        string path = $"Assets/Scripts/Game/Ui/{mFloder}/{mPanelText.value}";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        if (File.Exists($"{path}/{mPanelText.value}.cs"))
        {
            EditorUtility.DisplayDialog("Error", $"脚本已存在：{$"{path}/{mPanelText.value}.cs"}", "OK");
        }
        else
        {
            File.WriteAllText($"{path}/{mPanelText.value}.cs", mBuilder.ToString());
        }
    }
}
