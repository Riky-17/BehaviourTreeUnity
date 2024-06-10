using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees
{
    public abstract class ParentNode : BehaviourTreeNode
    {
        protected List<BehaviourTreeNode> Children {get; private set;}

        // public ParentNode(BehaviourTreeNode child)
        // {
        //     Children = new();
        //     AttachChild(child);
        // }

        // public ParentNode(params BehaviourTreeNode[] children)
        // {
        //     Children = new();
        //     foreach (BehaviourTreeNode child in children)
        //         AttachChild(child);
        // }
        
        // void AttachChild(BehaviourTreeNode child)
        // {
        //     child.parent = this;
        //     Children.Add(child);
        // }

        public override void ChildAwake()
        {
            foreach (BehaviourTreeNode child in Children)
                child.ChildAwake();
        }

        public override void ChildStart()
        {
            foreach (BehaviourTreeNode child in Children)
                child.ChildStart();
        }

        public override void ChildEnable()
        {
            foreach (BehaviourTreeNode child in Children)
                child.ChildEnable();
        }

        public override void ChildDisable()
        {
            foreach (BehaviourTreeNode child in Children)
                child.ChildDisable();
        }

        public override List<string> PopulateChildren(BehaviourTreeGraph graph)
        {
            Children = graph.GetNodes(ID);

            if (Children == null || Children.Count == 0)
                return null;

            List<string> childrenID = new();
            
            foreach (BehaviourTreeNode node in Children)
                childrenID.Add(node.ID);

            return childrenID;
        }
    }
}