using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeController : MonoBehaviour
{
    [SerializeField] private BehaviourTree _tree;
    public BehaviourTree tree { get => _tree; private set => _tree = value; }
    private Context context;

    void Start()
    {
        context = CreateBehaviourTreeContext();
        tree = tree.Clone();
    }

    void Update()
    {
        tree?.Update();
    }

    private void OnDrawGizmosSelected()
    {
        if (!tree)
        {
            return;
        }

        BehaviourTree.Traverse(tree.rootNode, (n) =>
        {
            if (n.drawGizmos)
            {
                n.OnDrawGizmos();
            }
        });
    }

    private Context CreateBehaviourTreeContext()
    {
        return Context.CreateFromGameObject(gameObject);
    }
}
