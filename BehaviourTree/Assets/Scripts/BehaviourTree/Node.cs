using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public enum NodeStates 
    {
        Success,
        Failure,
        Running
    }

    public abstract class Node
    {
        public Node parent;

        public virtual NodeStates Evaluate() => NodeStates.Failure;
    }
}