using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourTree.Editor
{
    public class BT_EditorNode : Node
    {
        public BehaviourTreeNode Node {get; private set;}

        public BT_EditorNode(BehaviourTreeNode node)
        {
            Node = node;
            AddToClassList("behaviour-tree-node");

            Type type = node.GetType();
            title = type.Name;
            NodeInfoAttribute nodeInfo = type.GetCustomAttribute<NodeInfoAttribute>();
            
            string[] depths = nodeInfo.Path.Split('/');

            foreach (string depth in depths)
                AddToClassList(depth.ToLower().Replace(' ', '-'));   

            name = type.Name;
        }

        public void SavePosition() => Node.SetPosition(GetPosition());
    }
}
