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

        [SerializeField] Rect position;
        [SerializeField] string id; 

        public Rect Position => position;
        public string Id => id;
        

        public BehaviourTreeNode() => NewGuid();

        public void SetPosition(Rect rect) => position = rect;
        public void NewGuid() => id = Guid.NewGuid().ToString();
    }
}