using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Selector : ParentNode
    {
        public Selector(Node child) : base(child) {}
        public Selector(params Node[] children) : base(children) {}

        public override NodeStates ChildUpdate()
        {
            if(Children == null || Children.Count == 0)
                return NodeStates.Failure;

            foreach (Node child in Children)
            {
                switch(child.ChildUpdate())
                {
                    case NodeStates.Failure:
                        continue;
                    case NodeStates.Success:
                        return NodeStates.Success;
                    case NodeStates.Running:
                        return NodeStates.Running;
                    default:
                        return NodeStates.Failure;
                }
            }
            return NodeStates.Failure;
        }
    }
}