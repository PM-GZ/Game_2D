using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GridEditor : EditorWindow
{
    [SerializeField]
    private VisualElement mRoot;
    private VisualElement mLeft;
    private VisualElement mRight;

    [MenuItem("Tools/GridEditor")]
    public static void ShowExample()
    {
        GridEditor wnd = GetWindow<GridEditor>();
        wnd.titleContent = new GUIContent("GridEditor");
    }

    public void CreateGUI()
    {
        mRoot = rootVisualElement;
        mRoot.style.flexDirection = FlexDirection.Row;

        Init();
    }

    private void Init()
    {
        InitLeft();
        InitRight();
    }

    private void InitLeft()
    {
        mLeft = new VisualElement();
        mLeft.style.width = 300;
        mRoot.Add(mLeft);

        var title = new Label("Inspector");
        title.style.fontSize = 20;
        title.style.unityFontStyleAndWeight = FontStyle.Bold;
        VisualElement titleContiner = new VisualElement();
        titleContiner.style.flexGrow = 1;
        titleContiner.style.alignSelf = Align.Center;
        titleContiner.Add(title);
        mLeft.Add(titleContiner);
    }

    private void InitRight()
    {
        mRight = new VisualElement();
        mRight.style.flexGrow = 1;
        mRoot.Add(mRight);

        BehaviourTreeView treeView = new BehaviourTreeView();
        treeView.style.flexGrow = 1;
        mRight.Add(treeView);
    }

    private T CreateElement<T>() where T : VisualElement
    {
        return default;
    }
}
