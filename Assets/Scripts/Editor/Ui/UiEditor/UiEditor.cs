using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class UiEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    private VisualElement mRoot;
    private Button mCutdown;
    private Button mAdd;
    private Button mSave;
    private ScrollView mConfigList;
    private VisualElement mPropertys;
    private ObjectField mPrefab;
    private EnumField mPanelLevel;
    private Toggle mForever;
    private EnumField mRedDotType;
    private ObjectField mAudioClip;

    private List<UiPanelSOConfig> mUiConfigList = new();
    private int mCurIndex;
    private string mFloder;

    [MenuItem("Tools/UiEditor")]
    public static void ShowExample()
    {
        UiEditor wnd = GetWindow<UiEditor>();
        wnd.titleContent = new GUIContent("UiEditor");
    }


    public void CreateGUI()
    {
        mRoot = rootVisualElement;
        mRoot.Clear();
        mRoot.Add(m_VisualTreeAsset.CloneTree());
        mUiConfigList.Clear();

        FindComponents();
        Init();
    }

    private void OnFocus()
    {
        CreateGUI();
    }

    private void FindComponents()
    {
        mCutdown = mRoot.Q<Button>("Cutdown");
        mAdd = mRoot.Q<Button>("Add");
        mSave = mRoot.Q<Button>("Save");
        mConfigList = mRoot.Q<ScrollView>("ConfigList");
        mPropertys = mRoot.Q<VisualElement>("Propertys");
        mPrefab = mRoot.Q<ObjectField>("Prefab");
        mPanelLevel = mRoot.Q<EnumField>("PanelLevel");
        mForever = mRoot.Q<Toggle>("Forever");
        mRedDotType = mRoot.Q<EnumField>("RedDotType");
        mAudioClip = mRoot.Q<ObjectField>("AudioClip");

        mCutdown.clicked += OnCutdownClick;
        mAdd.clicked += OnAddClick;
        mSave.clicked += OnSaveClick;
    }

    private void Init()
    {
        string[] files = Directory.GetFiles("Assets/GameAssets/Data/SOData/UiSOData", "*.asset", SearchOption.AllDirectories);
        int i = 0;
        foreach (string file in files)
        {
            int index = i;
            string path = file.Replace(Application.dataPath, "Assets");
            var config = AssetDatabase.LoadAssetAtPath<UiPanelSOConfig>(path);
            mUiConfigList.Add(config);

            Button element = new Button();
            element.text = config.name;
            element.clicked += () => { OnConfigSelectChanged(index); };
            mConfigList.hierarchy.Add(element);
            i++;
        }
    }

    private void OnConfigSelectChanged(int index)
    {
        mCurIndex = index;
        mPropertys.style.display = DisplayStyle.Flex;
        var config = mUiConfigList[index];
        mPrefab.value = config.PanelPrefab;
        mPanelLevel.value = config.Level;
        mForever.value = config.Forever;
        mRedDotType.value = config.RedDotType;
        mAudioClip.value = config.Bgm;
    }

    private void OnCutdownClick()
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
        if (EditorUtility.DisplayDialog("提示", "此操作将会删除 SO、脚本以及预制体！！！", "确认", "取消"))
        {
            var config = mUiConfigList[mCurIndex];
            string name = config.name.Substring(0, config.name.IndexOf("SO"));
            string prefabPath = $"Assets/GameAssets/Ui/{mFloder}/{name}/{name}.prefab";
            string scriptPath = $"Assets/Scripts/Game/Ui/{mFloder}/{name}/{name}.cs";
            string SOPath = $"Assets/GameAssets/Data/SOData/UiSOData/{config.name}.asset";

            string floderPath = Directory.GetParent(prefabPath).FullName;
            int fileCount = 0;
            if (File.Exists(prefabPath))
            {
                fileCount = Directory.GetFiles(floderPath, "*.prefab", SearchOption.AllDirectories).Length;
                if (fileCount == 1)
                {
                    File.Delete($"{Directory.GetParent(floderPath).FullName}/{name}.meta");
                    Directory.Delete(floderPath, true);
                }
                else
                {
                    File.Delete(prefabPath);
                }
            }
            floderPath = Directory.GetParent(scriptPath).FullName;
            if (File.Exists(scriptPath))
            {
                fileCount = Directory.GetFiles(floderPath, "*.cs", SearchOption.AllDirectories).Length;
                if (fileCount == 1)
                {
                    File.Delete($"{Directory.GetParent(floderPath).FullName}/{name}.meta");
                    Directory.Delete(floderPath, true);
                }
                else
                {
                    File.Delete(scriptPath);
                }
            }
            if (File.Exists(SOPath))
            {
                File.Delete(SOPath);
            }
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("提示", "删除完成", "确认");
        }
    }

    private void OnAddClick()
    {
        CreateUiEditor wnd = GetWindow<CreateUiEditor>();
        wnd.titleContent = new GUIContent("Create Ui");
    }

    private void OnSaveClick()
    {
        var config = mUiConfigList[mCurIndex];
        config.PanelPrefab = mPrefab.value as GameObject;
        config.Level = (PanelLevel)mPanelLevel.value;
        config.Forever = mForever.value;
        config.RedDotType = (RedDotSystem.RedDotType)mRedDotType.value;
        config.Bgm = mAudioClip.value as AudioClip;
        EditorUtility.SetDirty(config);
    }
}
