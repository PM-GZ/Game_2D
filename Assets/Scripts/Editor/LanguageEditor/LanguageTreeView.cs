using UnityEditor.IMGUI.Controls;

public class LanguageTreeView : TreeView
{
    public LanguageTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
    {
        Reload();
    }

    protected override TreeViewItem BuildRoot()
    {
        var root = new TreeViewItem(0, -1, "Root");


        return root;
    }
}
