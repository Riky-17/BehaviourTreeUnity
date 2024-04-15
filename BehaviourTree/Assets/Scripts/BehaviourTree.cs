using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourTree : MonoBehaviour
{
    protected Node root = null;

    void Awake() => SetUpTree();

    void Update() => root?.Evaluate();

    public abstract void SetUpTree();
}
