using System;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class TerrainSplit : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    private ObjectField _terrain;
    private UnsignedIntegerField _minWidth;
    private UnsignedIntegerField _minHeight;
    private ObjectField _floder;
    private Button _split;
    private Button _saveConfig;
    private HelpBox box;

    [MenuItem("Tools/TerrainSplit")]
    public static void ShowExample()
    {
        TerrainSplit wnd = GetWindow<TerrainSplit>();
        wnd.titleContent = new GUIContent("TerrainSplit");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        root.Add(m_VisualTreeAsset.CloneTree());

        _terrain = root.Q<ObjectField>("Terrain");
        _minWidth = root.Q<UnsignedIntegerField>("MinWidth");
        _minHeight = root.Q<UnsignedIntegerField>("MinHeight");
        _floder = root.Q<ObjectField>("Floder");
        _split = root.Q<Button>("Split");
        _saveConfig = root.Q<Button>("SaveConfig");

        box = new HelpBox();
        box.style.display = DisplayStyle.None;
        root.Add(box);

        _terrain.RegisterValueChangedCallback(OnTerrainChanged);
        _minWidth.RegisterValueChangedCallback(OnMinWidthValueChagned);
        _minHeight.RegisterValueChangedCallback(OnMinHeightValueChagned);
        _floder.RegisterValueChangedCallback(OnFloderChanged);
        _split.clicked += OnSplitClick;
        _saveConfig.clicked += OnSaveConfigClick;

        SetModuleDisplay(DisplayStyle.None);
    }

    private void OnTerrainChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        if (evt.newValue != null)
        {
            SetModuleDisplay(DisplayStyle.Flex);
        }
        else
        {
            SetModuleDisplay(DisplayStyle.None);
        }
    }

    private void OnMinWidthValueChagned(ChangeEvent<uint> evt)
    {
        if (evt.newValue <= 0)
        {
            _minWidth.value = 1;
        }
    }

    private void OnMinHeightValueChagned(ChangeEvent<uint> evt)
    {
        if (evt.newValue <= 0)
        {
            _minHeight.value = 1;
        }
    }

    private void OnFloderChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        if (evt.newValue != null)
        {
            box.style.display = DisplayStyle.None;
        }
    }

    private void OnSplitClick()
    {
        if (_floder.value == null)
        {
            SetHelpBox("选择保存路径！", HelpBoxMessageType.Error, DisplayStyle.Flex);
        }
        else
        {
            box.style.display = DisplayStyle.None;
            SplitTerrain();
        }
    }

    private void OnSaveConfigClick()
    {
        if (_floder.value == null)
        {
            SetHelpBox("选择地图块文件夹！", HelpBoxMessageType.Error, DisplayStyle.Flex);
        }
        else
        {
            box.style.display = DisplayStyle.None;
            SaveConfig();
        }
    }

    private void SetModuleDisplay(DisplayStyle display)
    {
        _minWidth.style.display = display;
        _minHeight.style.display = display;
        _floder.style.display = display;
        _split.style.display = display;
        _saveConfig.style.display = display;
    }

    private void SetHelpBox(string content, HelpBoxMessageType msgType, DisplayStyle display)
    {
        box.text = content;
        box.messageType = msgType;
        box.style.display = display;
    }

    private void SplitTerrain()
    {
        Terrain terrain = _terrain.value as Terrain;
        float maxTerrainSide = Mathf.Max(terrain.terrainData.size.x, terrain.terrainData.size.z);
        int blockCount = Mathf.CeilToInt(maxTerrainSide / _minWidth.value);
        blockCount *= blockCount;
    }

    private void SaveConfig()
    {
        string path = AssetDatabase.GetAssetPath(_floder.value);
        string[] files = Directory.GetFiles(path, "*.asset", SearchOption.AllDirectories);
        if (files == null)
        {
            SetHelpBox("文件夹中没有地形资源", HelpBoxMessageType.Error, DisplayStyle.Flex);
            return;
        }

        foreach (var file in files)
        {

        }
    }
}
