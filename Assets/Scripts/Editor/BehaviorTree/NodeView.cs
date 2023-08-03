using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeView> onNodeSelected;
    public Node node;
    public Port inputPort;
    public Port outputPort;

    public NodeView(Node node) : base(AssetDatabase.GetAssetPath(BehaviourTreeSettings.GetOrCreateSettings().nodeXml))
    {
        this.node = node;
        this.node.name = node.GetType().Name;
        this.title = node.name.Replace("(Clone)", "").Replace("Node", "");
        this.viewDataKey = node.guid;

        style.left = node.posistion.x;
        style.top = node.posistion.y;

        CreateInputPorts();
        CreateOutputPorts();
        SetupClasses();
        SetupDataBinding();
    }

    private void SetupDataBinding()
    {
        Label descriptionLabel = this.Q<Label>("description");
        descriptionLabel.bindingPath = "description";
        descriptionLabel.Bind(new SerializedObject(node));
    }

    private void SetupClasses()
    {
        if (node is ActionNode)
        {
            AddToClassList("action");
        }
        else if (node is CompositeNode)
        {
            AddToClassList("composite");
        }
        else if (node is DecoratorNode)
        {
            AddToClassList("decorator");
        }
        else if (node is RootNode)
        {
            AddToClassList("root");
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(node, "Behaviour Tree (Set Position)");
        node.posistion.x = newPos.x;
        node.posistion.y = newPos.y;
        EditorUtility.SetDirty(node);
    }

    private void CreateInputPorts()
    {
        if (node is ActionNode)
        {
            inputPort = new NodePort(Direction.Input, Port.Capacity.Single);
        }
        else if (node is CompositeNode)
        {
            inputPort = new NodePort(Direction.Input, Port.Capacity.Single);
        }
        else if (node is DecoratorNode)
        {
            inputPort = new NodePort(Direction.Input, Port.Capacity.Single);
        }
        else if (node is RootNode)
        {

        }

        if (inputPort != null)
        {
            inputPort.portName = "";
            inputPort.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(inputPort);
        }
    }

    private void CreateOutputPorts()
    {
        if (node is ActionNode)
        {

        }
        else if (node is CompositeNode)
        {
            outputPort = new NodePort(Direction.Output, Port.Capacity.Multi);
        }
        else if (node is DecoratorNode)
        {
            outputPort = new NodePort(Direction.Output, Port.Capacity.Single);
        }
        else if (node is RootNode)
        {
            outputPort = new NodePort(Direction.Output, Port.Capacity.Single);
        }

        if (outputPort != null)
        {
            outputPort.portName = "";
            outputPort.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(outputPort);
        }
    }

    public override void OnSelected()
    {
        base.OnSelected();
        onNodeSelected?.Invoke(this);
    }

    public void SortChildren()
    {
        if (node is CompositeNode composite)
        {
            composite.children.Sort(SortByHorizontalPosition);
        }
    }

    private int SortByHorizontalPosition(Node left, Node right)
    {
        return left.posistion.x < right.posistion.x ? -1 : 1;
    }

    public void UpdateState()
    {
        RemoveFromClassList("running");
        RemoveFromClassList("failure");
        RemoveFromClassList("success");

        if (!Application.isPlaying) return;

        switch (node.state)
        {
            case Node.State.Running:
                if (node.started)
                {
                    AddToClassList("running");
                }
                break;
            case Node.State.Failure:
                AddToClassList("failure");
                break;
            case Node.State.Success:
                AddToClassList("success");
                break;
        }
    }
}
