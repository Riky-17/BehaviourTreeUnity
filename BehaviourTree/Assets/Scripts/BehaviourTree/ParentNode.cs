using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

namespace BehaviourTree
{
    public abstract class ParentNode : Node
    {
        protected List<Node> Children {get; private set;}

        public ParentNode(Node child)
        {
            Children = new();
            AttachChild(child);
        }

        public ParentNode(params Node[] children)
        {
            this.Children = new();
            foreach (Node child in children)
                AttachChild(child);
        }
        
        void AttachChild(Node child)
        {
            child.parent = this;
            Children.Add(child);
        }
    }
}