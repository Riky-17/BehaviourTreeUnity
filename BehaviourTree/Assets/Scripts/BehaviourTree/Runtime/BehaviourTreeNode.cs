using System;
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

    [Serializable]
    public abstract class BehaviourTreeNode
    {
        public BehaviourTreeNode parent;

        public virtual void ChildAwake() {}
        public virtual void ChildStart() {}
        public virtual void ChildEnable() {}
        public virtual void ChildDisable() {}
        public virtual NodeStates ChildUpdate() => NodeStates.Failure;

        //graph methods and fields

        public Rect Position {get; private set;}
        public string Id {get; private set;}

        public BehaviourTreeNode() => NewGuid();

        public void SetPosition(Rect rect) => Position = rect;
        public void NewGuid() => Id = Guid.NewGuid().ToString();
    }
}