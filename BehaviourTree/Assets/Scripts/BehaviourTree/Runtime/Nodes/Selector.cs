using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees
{
    [NodeInfo("Parent Node/Selector", HasMultipleChildren = true)]
    public class Selector : ParentNode
    {
        public Selector() {}

        public override NodeStates ChildUpdate()
        {
            if(Children == null || Children.Count == 0)
                return NodeStates.Failure;

            foreach (BehaviourTreeNode child in Children)
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

        public override List<string> PopulateChildren(BehaviourTreeGraph graph)
        {
            Debug.Log("selector");

            return base.PopulateChildren(graph);
        }
    }
}