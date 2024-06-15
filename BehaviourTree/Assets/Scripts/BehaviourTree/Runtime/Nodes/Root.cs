using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees
{
    [NodeInfo("", false)]
    public class Root : ParentNode
    {
        public Root() {}

        public override NodeStates ChildUpdate()
        {
            if(Children.Count == 0)
                return NodeStates.Failure;

            return Children[0].ChildUpdate();
        }

        public override List<string> PopulateChildren(BehaviourTreeGraph graph)
        {
            Debug.Log("Root");

            return base.PopulateChildren(graph);
        }
    }
}
