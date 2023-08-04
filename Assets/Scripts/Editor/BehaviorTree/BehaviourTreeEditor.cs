using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System.IO;

public class BehaviourTreeEditor : EditorWindow
{
    private BehaviourTreeView _treeView;
    private InspectorView _inspectorView;
    private BehaviourTree _tree;
    private IMGUIContainer _blackboardView;
    private ToolbarMenu _toolbarMenu;
    private TextField _treeNameField;
    private ObjectField _locationPathField;
    private Button _createNewTreeButton;
    private Button _openTreeButton;
    private VisualElement _overlay;
    private BehaviourTreeSettings _settings;

    private SerializedObject _treeObject;
    private SerializedProperty _blackboardProperty;

    [MenuItem("Tools/行为树")]
    public static void OpenWindow()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
        wnd.minSize = new Vector2(800, 600);
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (Selection.activeObject is BehaviourTree)
        {
            OpenWindow();
            return true;
        }
        return false;
    }

    private List<T> LoadAssets<T>() where T : UnityEngine.Object
    {
        string[] assetIds = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
        List<T> assets = new List<T>();
        foreach (var assetId in assetIds)
        {
            string path = AssetDatabase.GUIDToAssetPath(assetId);
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            assets.Add(asset);
        }
        return assets;
    }

    public void CreateGUI()
    {
        _settings = BehaviourTreeSettings.GetOrCreateSettings();

        VisualElement root = rootVisualElement;

        var visualTree = _settings.behaviourTreeXml;
        visualTree.CloneTree(root);

        var styleSheet = _settings.behaviourTreeStyle;
        root.styleSheets.Add(styleSheet);

        _treeView = root.Q<BehaviourTreeView>();
        _inspectorView = root.Q<InspectorView>();
        _blackboardView = root.Q<IMGUIContainer>();

        _treeView.onNodeSelected = OnNodeSelectedChanged;
        _blackboardView.onGUIHandler = () =>
        {
            if (_treeObject != null && _treeObject.targetObject != null)
            {
                _treeObject.Update();
                EditorGUILayout.PropertyField(_blackboardProperty);
                _treeObject.ApplyModifiedProperties();
            }
        };

        _toolbarMenu = root.Q<ToolbarMenu>();
        var behaviourTrees = LoadAssets<BehaviourTree>();
        behaviourTrees.ForEach(tree =>
        {
            _toolbarMenu.menu.AppendAction($"{tree.name}", (a) =>
            {
                Selection.activeObject = tree;
            });
        });
        _toolbarMenu.menu.AppendSeparator();
        _toolbarMenu.menu.AppendAction("New Tree...", (a) => CreateNewTree("NewBehaviorTree"));

        _treeNameField = root.Q<TextField>("TreeName");
        _locationPathField = root.Q<ObjectField>("LocationPath");
        _overlay = root.Q<VisualElement>("Overlay");
        _createNewTreeButton = root.Q<Button>("CreateButton");
        _openTreeButton = root.Q<Button>("OpenButton");

        _locationPathField.objectType = typeof(DefaultAsset);
        _locationPathField.allowSceneObjects = false;
        _locationPathField.RegisterValueChangedCallback(OnLocationPathChanged);
        _createNewTreeButton.clicked += () => CreateNewTree(_treeNameField.value);
        _openTreeButton.clicked += OpenTree;

        if (_tree == null)
        {
            OnSelectionChange();
        }
        else
        {
            SelectTree(_tree);
        }
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayerModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayerModeStateChanged;
    }

    private void OnSelectionChange()
    {
        EditorApplication.delayCall += () =>
        {
            BehaviourTree tree = Selection.activeObject as BehaviourTree;
            if (!tree && Selection.activeGameObject)
            {
                if (Selection.activeGameObject.TryGetComponent<BehaviourTreeController>(out var ctrl))
                {
                    tree = ctrl.tree;
                }
            }

            SelectTree(tree);
        };
    }


    private void OnInspectorUpdate()
    {
        _treeView?.UpdateNodeState();
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayerModeStateChanged;
    }

    private void OnPlayerModeStateChanged(PlayModeStateChange change)
    {
        switch (change)
        {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingPlayMode:
                break;
        }
    }

    private void OnNodeSelectedChanged(NodeView nodeView)
    {
        _inspectorView?.UpdateSelection(nodeView);
    }

    private void OnLocationPathChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        string path = AssetDatabase.GetAssetPath(evt.newValue);
        if (string.IsNullOrEmpty(path)) return;
    }

    private void CreateNewTree(string assetName)
    {
        if (_locationPathField.value is not DefaultAsset floder) return;

        if (!Directory.Exists(AssetDatabase.GetAssetPath(floder)))
        {
            EditorUtility.DisplayDialog("错误", "路径选择不正确！\n请选择文件夹", "确认");
            return;
        }

        string path = AssetDatabase.GetAssetPath(floder);
        path = Path.Combine(path, $"{assetName}.asset");
        if (File.Exists(path))
        {
            EditorUtility.DisplayDialog("错误", "文件重名，请更改名称再创建！", "确认");
            return;
        }

        BehaviourTree tree = ScriptableObject.CreateInstance<BehaviourTree>();
        tree.name = _treeNameField.ToString();
        AssetDatabase.CreateAsset(tree, path);
        AssetDatabase.SaveAssets();
        Selection.activeObject = tree;
        EditorGUIUtility.PingObject(tree);
    }

    private void OpenTree()
    {
        string path = EditorUtility.OpenFilePanel("选择行为树资产", "Assets/", "asset");
        if (string.IsNullOrEmpty(path)) return;

        path = path.Replace(Application.dataPath, "Assets");
        var tree = AssetDatabase.LoadAssetAtPath<BehaviourTree>(path);
        var id = tree.GetInstanceID();
        AssetDatabase.OpenAsset(id);

        _overlay.style.display = DisplayStyle.None;
    }

    void SelectTree(BehaviourTree newTree)
    {
        if (_treeView == null || !newTree)
            return;

        _tree = newTree;

        _overlay.style.visibility = Visibility.Hidden;

        _treeView.PopulateView(_tree);

        _treeObject = new SerializedObject(_tree);
        _blackboardProperty = _treeObject.FindProperty("blackboard");

        EditorApplication.delayCall += () =>
        {
            _treeView.FrameAll();
        };
    }
}
