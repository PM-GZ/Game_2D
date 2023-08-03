using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using System.Collections.Generic;


public class BehaviourTreeEditor : EditorWindow
{
    private BehaviourTreeView _treeView;
    private InspectorView _inspectorView;
    private BehaviourTree _tree;
    private IMGUIContainer _blackboardView;
    private ToolbarMenu _toolbarMenu;
    private TextField _treeNameField;
    private TextField _locationPathField;
    private Button _createNewTreeButton;
    private VisualElement _overlay;
    private BehaviourTreeSettings _settings;

    private SerializedObject _treeObject;
    private SerializedProperty _blackboardProperty;

    [MenuItem("Tools/ÐÐÎªÊ÷")]
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
        _locationPathField = root.Q<TextField>("LocationPath");
        _overlay = root.Q<VisualElement>("Overlay");
        _createNewTreeButton = root.Q<Button>("CreateButton");
        _createNewTreeButton.clicked += () => CreateNewTree(_treeNameField.value);

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
        EditorApplication.playModeStateChanged -= OnPlayerMOdeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayerMOdeStateChanged;
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
        EditorApplication.playModeStateChanged -= OnPlayerMOdeStateChanged;
    }

    private void OnPlayerMOdeStateChanged(PlayModeStateChange change)
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

    private void CreateNewTree(string assetName)
    {
        string path = System.IO.Path.Combine(_locationPathField.value, $"{assetName}.asset");
        BehaviourTree tree = ScriptableObject.CreateInstance<BehaviourTree>();
        tree.name = _treeNameField.ToString();
        AssetDatabase.CreateAsset(tree, path);
        AssetDatabase.SaveAssets();
        Selection.activeObject = tree;
        EditorGUIUtility.PingObject(tree);
    }

    void SelectTree(BehaviourTree newTree)
    {

        if (_treeView == null)
        {
            return;
        }

        if (!newTree)
        {
            return;
        }

        _tree = newTree;

        _overlay.style.visibility = Visibility.Hidden;

        if (Application.isPlaying)
        {
            _treeView.PopulateView(_tree);
        }
        else
        {
            _treeView.PopulateView(_tree);
        }


        _treeObject = new SerializedObject(_tree);
        _blackboardProperty = _treeObject.FindProperty("blackboard");

        EditorApplication.delayCall += () =>
        {
            _treeView.FrameAll();
        };
    }
}
