using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Children = new();
            foreach (Node child in children)
                AttachChild(child);
        }
        
        void AttachChild(Node child)
        {
            child.parent = this;
            Children.Add(child);
        }

        public override void ChildAwake()
        {
            foreach (Node child in Children)
                child.ChildAwake();
        }

        public override void ChildStart()
        {
            foreach (Node child in Children)
                child.ChildStart();
        }

        public override void ChildEnable()
        {
            foreach (Node child in Children)
                child.ChildEnable();
        }

        public override void ChildDisable()
        {
            foreach (Node child in Children)
                child.ChildDisable();
        }
    }
}