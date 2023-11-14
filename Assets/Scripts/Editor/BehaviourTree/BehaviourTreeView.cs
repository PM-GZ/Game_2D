using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;



public class BehaviourTreeView : GraphView
{
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }
    public BehaviourTreeView()
    {
        Insert(0, new GridBackground());
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/GridEditor/GridEditor.uss");
        styleSheets.Add(styleSheet);

        this.StretchToParentSize();
        this.AddManipulator(new ContentZoomer()); 
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        AddStartNode();
    }

    public void AddStartNode()
    {
        
    }

    public BaseNode AddNode(string name)
    {
        BaseNode node = new BaseNode();
        node.title = node.name = name;
        node.SetPosition(new Rect(Vector2.one, Vector2.one));

        AddElement(node);
        return node;
    }
}
