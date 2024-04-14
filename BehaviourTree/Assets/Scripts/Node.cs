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
    Node parent;
    protected List<Node> children;
    NodeStates state;
    Dictionary<string, object> data;

    public Node() => children = null;

    public Node(params Node[] children)
    {
        this.children = new();
        foreach (Node child in children)
            AttachChild(child);
    }

    void AttachChild(Node child)
    {
        child.parent = this;
        children.Add(child);
    }

    public object GetData(string key)
    {
        if (data.TryGetValue(key, out object value))
            return value;

        if(parent != null)
            return parent.GetData(key);
        else
            return null;
    }

    public void ClearData(string key)
    {
        if (data.ContainsKey(key))
            data.Remove(key);

        parent?.ClearData(key);
    }

    public void SetData(string key, object value) => data[key] = value;

    public virtual NodeStates Evaluate() => NodeStates.Failure;
}
