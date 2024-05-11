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
        public List<Port> Ports {get; private set;}

        public BT_EditorNode(BehaviourTreeNode node)
        {
            Node = node;
            Ports = new();
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
            Port inputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(PortTypes.FlowPort));
            inputPort.portName = "Input";
            inputPort.tooltip = "Flow Input";
            Ports.Add(inputPort);
            inputContainer.Add(inputPort);
        }

        private void CreateOutputPort(NodeInfoAttribute nodeInfo)
        {
            Port.Capacity capacity = nodeInfo.HasMultipleOutput ? Port.Capacity.Multi : Port.Capacity.Single;

            Port outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, capacity, typeof(PortTypes.FlowPort));
            outputPort.portName = "Output";
            outputPort.tooltip = "Flow Output";
            Ports.Add(outputPort);
            outputContainer.Add(outputPort);
        }

        public void SavePosition() => Node.SetPosition(GetPosition());
    }
}
