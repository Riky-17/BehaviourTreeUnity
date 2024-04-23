using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Sequence : Node
    {
        public Sequence() : base() {}
        public Sequence(params Node[] children) : base(children) {}

        public override NodeStates Evaluate()
        {
            if(children == null || children.Count == 0)
                return NodeStates.Failure;

            foreach (Node child in children)
            {
                switch(child.Evaluate())
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