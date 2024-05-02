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

        public virtual void ChildAwake() {}
        public virtual void ChildStart() {}
        public virtual void ChildEnable() {}
        public virtual void ChildDisable() {}
        public virtual NodeStates ChildUpdate() => NodeStates.Failure;
    }
}