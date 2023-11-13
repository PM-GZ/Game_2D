using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;



public class BehaviourTreeView : GraphView
{
    //public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }
    public BehaviourTreeView()
    {
        Insert(0, new GridBackground());
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/GridEditor/GridEditor.uss");
        styleSheets.Add(styleSheet);

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        this.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
    }
}
