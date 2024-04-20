using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeStates 
{
    Success,
    Failure,
    Running
}

public class Node
{
    public Node Parent { get; private set; }
    protected List<Node> children;

    public Node() => children = null;

    public Node(params Node[] children)
    {
        this.children = new();
        foreach (Node child in children)
            AttachChild(child);
    }

    protected void AttachChild(Node child)
    {
        child.Parent = this;
        children.Add(child);
    }

    public virtual NodeStates Evaluate() => NodeStates.Failure;
}
