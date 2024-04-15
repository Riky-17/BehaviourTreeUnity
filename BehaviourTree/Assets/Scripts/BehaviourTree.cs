using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourTree : MonoBehaviour
{
    Node root = null;

    void Awake()
    {
        SetUpTree();
    }

    void SetUpTree()
    {
        root = new Selector();
    }

    void Update()
    {
        root?.Evaluate();
    }
}
