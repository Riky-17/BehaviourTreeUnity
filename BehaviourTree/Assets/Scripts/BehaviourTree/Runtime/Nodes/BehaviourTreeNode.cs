using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees
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

        public virtual List<string> PopulateChildren(BehaviourTreeGraph graph) => null;

        //graph methods and fields

        [SerializeField] Rect position;
        [SerializeField] string id; 

        public Rect Position => position;
        public string ID => id;
        
        public BehaviourTreeNode() => NewGuid();

        public void SetPosition(Rect rect) => position = rect;
        public void NewGuid() => id = Guid.NewGuid().ToString();
    }
}