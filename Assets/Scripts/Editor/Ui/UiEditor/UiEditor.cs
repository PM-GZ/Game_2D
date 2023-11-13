using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class UiEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    private VisualElement mRoot;
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

    [MenuItem("Tools/UiEditor")]
    public static void ShowExample()
    {
        UiEditor wnd = GetWindow<UiEditor>();
        wnd.titleContent = new GUIContent("UiEditor");
    }


    public void CreateGUI()
    {
        mRoot = rootVisualElement;
        mRoot.Add(m_VisualTreeAsset.CloneTree());

        FindComponents();
        Init();
    }

    private void FindComponents()
    {
        mSave = mRoot.Q<Button>("Save");
        mConfigList = mRoot.Q<ScrollView>("ConfigList");
        mPropertys = mRoot.Q<VisualElement>("Propertys");
        mPrefab = mRoot.Q<ObjectField>("Prefab");
        mPanelLevel = mRoot.Q<EnumField>("PanelLevel");
        mForever = mRoot.Q<Toggle>("Forever");
        mRedDotType = mRoot.Q<EnumField>("RedDotType");
        mAudioClip = mRoot.Q<ObjectField>("AudioClip");

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
