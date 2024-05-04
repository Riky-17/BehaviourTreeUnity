using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourTree.Editor
{
    public class BT_EditorNode : Node
    {
        public BT_EditorNode()
        {
            AddToClassList("behaviour-tree-node");
        }
    }
}
