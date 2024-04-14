using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
   public override NodeStates Evaluate()
    {
        if(children == null || children.Count == 0)
            return NodeStates.Failure;

        foreach (Node child in children)
        {
            switch(child.Evaluate())
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
