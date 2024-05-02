using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    [NodeInfo]
    public class Sequence : ParentNode
    {
        public Sequence(Node child) : base(child) {}
        public Sequence(params Node[] children) : base(children) {}

        public override NodeStates ChildUpdate()
        {
            if(Children == null || Children.Count == 0)
                return NodeStates.Failure;

            foreach (Node child in Children)
            {
                switch(child.ChildUpdate())
                {
                    case NodeStates.Failure:
                        return NodeStates.Failure;
                    case NodeStates.Success:
                        continue;
                    case NodeStates.Running:
                        return NodeStates.Running;
                    default:
                        return NodeStates.Failure;
                }
            }
            return NodeStates.Success;
        }
    }
}