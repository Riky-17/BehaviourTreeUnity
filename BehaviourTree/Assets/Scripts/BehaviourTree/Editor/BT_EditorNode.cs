using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTrees.Editor
{
    public class BT_EditorNode : Node
    {
        public BehaviourTreeNode Node {get; private set;}
        public Port InputPort {get; private set;}
        public Port OutputPort {get; private set;}

        public BT_EditorNode(BehaviourTreeNode node) : base("Assets/Scripts/BehaviourTree/Editor/Style/BT_GraphNode.uxml")
        {
            Node = node;
            capabilities ^= Capabilities.Snappable;
            
            AddToClassList("behaviour-tree-node");

            Type type = node.GetType();
            title = type.Name;
            NodeInfoAttribute nodeInfo = type.GetCustomAttribute<NodeInfoAttribute>();
            
            string[] depths = nodeInfo.Path.Split('/');

            foreach (string depth in depths)
                AddToClassList(depth.ToLower().Replace(' ', '-'));   

            name = type.Name;

            if(nodeInfo.HasInput)
                CreateInputPort();
            if(nodeInfo.HasOutput)
                CreateOutputPort(nodeInfo);
        }

        private void CreateInputPort()
        {
            InputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(PortTypes.FlowPort));
            InputPort.portName = "";
            InputPort.tooltip = "The Parent of this node";
            inputContainer.Add(InputPort);
        }

        private void CreateOutputPort(NodeInfoAttribute nodeInfo)
        {
            Port.Capacity capacity = nodeInfo.HasMultipleOutput ? Port.Capacity.Multi : Port.Capacity.Single;

            OutputPort = InstantiatePort(Orientation.Vertical, Direction.Output, capacity, typeof(PortTypes.FlowPort));
            OutputPort.portName = "";
            OutputPort.tooltip = nodeInfo.HasMultipleOutput ? "The Children of this node" : "The Child of this node";
            outputContainer.Add(OutputPort);
        }

        public void SetPosition() => Node.SetPosition(GetPosition());
    }
}
