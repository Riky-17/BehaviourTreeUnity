using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees
{
    [NodeInfo("Parent Node/Sequence", HasMultipleOutput = true)]
    public class Sequence : ParentNode
    {
        public Sequence() {}

        public override NodeStates ChildUpdate()
        {
            if(Children == null || Children.Count == 0)
                return NodeStates.Failure;

            foreach (BehaviourTreeNode child in Children)
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

        public override List<string> PopulateChildren(BehaviourTreeGraph graph)
        {
            Debug.Log("sequence");

            return base.PopulateChildren(graph);
        }
    }
}