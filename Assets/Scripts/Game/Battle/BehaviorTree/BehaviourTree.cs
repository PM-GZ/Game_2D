using System;
using System.Collections.Generic;
using UnityEngine;
using State = Node.State;
#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
    public Node rootNode;
    public State state = State.Running;
    public List<Node> nodes = new();
    public Blackboard blackboard = new();

    public State Update()
    {
        if (rootNode.state == State.Running)
        {
            state = rootNode.Update();
        }
        return state;
    }

#if UNITY_EDITOR
    public Node CreateNode(Type type)
    {
        Node node = ScriptableObject.CreateInstance(type) as Node;
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();

        Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
        nodes.Add(node);

        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(node, this);
        }
        Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree(CreateNode)");

        AssetDatabase.SaveAssets();
        return node;
    }

    public void DeleteNode(Node node)
    {
        Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");
        nodes.Remove(node);

        //AssetDatabase.RemoveObjectFromAsset(node);
        Undo.DestroyObjectImmediate(node);

        AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node node)
    {
        if (parent is DecoratorNode decorator)
        {
            Undo.RecordObject(decorator, "Behaviour Tree (AddChild)");
            decorator.child = node;
            EditorUtility.SetDirty(decorator);
        }

        if (parent is RootNode rootNode)
        {
            Undo.RecordObject(rootNode, "Behaviour Tree (AddChild)");
            rootNode.child = node;
            EditorUtility.SetDirty(rootNode);
        }

        if (parent is CompositeNode composite)
        {
            Undo.RecordObject(composite, "Behaviour Tree (AddChild)");
            composite.children.Add(node);
            EditorUtility.SetDirty(composite);
        }
    }

    public void RemoveChild(Node parent, Node node)
    {
        if (parent is DecoratorNode decorator)
        {
            Undo.RecordObject(decorator, "Behaviour Tree (RemoveChild)");
            decorator.child = null;
            EditorUtility.SetDirty(decorator);
        }

        if (parent is RootNode rootNode)
        {
            Undo.RecordObject(rootNode, "Behaviour Tree (RemoveChild)");
            rootNode.child = null;
            EditorUtility.SetDirty(rootNode);
        }

        if (parent is CompositeNode composite)
        {
            Undo.RecordObject(composite, "Behaviour Tree (RemoveChild)");
            composite.children.Remove(node);
            EditorUtility.SetDirty(composite);
        }
    }
#endif

    public static List<Node> GetChildren(Node parent)
    {
        List<Node> children = new List<Node>();
        if (parent is DecoratorNode decorator && decorator.child != null)
        {
            children.Add(decorator.child);
        }

        if (parent is RootNode rootNode && rootNode.child != null)
        {
            children.Add(rootNode.child);
        }

        if (parent is CompositeNode composite)
        {
            children = composite.children;
        }

        return children;
    }

    public static void Traverse(Node node, Action<Node> visiter)
    {
        if (node)
        {
            visiter.Invoke(node);
            var children = GetChildren(node);
            children.ForEach((n) => Traverse(n, visiter));
        }
    }

    public BehaviourTree Clone()
    {
        BehaviourTree tree = Instantiate(this);
        tree.rootNode = tree.rootNode.Clone();
        tree.nodes = new List<Node>();
        Traverse(tree.rootNode, (n) =>
        {
            tree.nodes.Add(n);
        });

        return tree;
    }

    public void Bind(Context context)
    {
        Traverse(rootNode, node => {
            node.context = context;
            node.blackboard = blackboard;
        });
    }
}
