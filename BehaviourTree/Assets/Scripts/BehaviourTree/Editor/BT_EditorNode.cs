using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTrees.Editor
{
    public class BT_EditorNode : Node
    {
        BT_Window window;
        SerializedProperty serializedNode;
        SerializedObject serializedGraph;

        Label nodeLabel;
        List<PropertyField> fields;

        [SerializeField] public BehaviourTreeNode Node {get; private set;}

        public Port InputPort {get; private set;}
        public Port OutputPort {get; private set;}

        public BT_EditorNode(BehaviourTreeNode node, SerializedObject serializedGraph, BT_Window window) : base("Assets/Scripts/BehaviourTree/Editor/Style/BT_GraphNode.uxml")
        {
            this.window = window;
            this.serializedGraph = serializedGraph;
            Node = node;
            
            //removing broken capabilities
            capabilities ^= Capabilities.Snappable | Capabilities.Copiable;

            //making the root node undeletable
            if(node is Root)
                capabilities ^= Capabilities.Deletable;
            
            AddToClassList("behaviour-tree-node");

            Type type = node.GetType();
            title = type.Name.AddSpaces();

            NodeInfoAttribute nodeInfo = type.GetCustomAttribute<NodeInfoAttribute>();
            
            string[] depths = nodeInfo.Path.Split('/');

            foreach (string depth in depths)
                AddToClassList(depth.ToLower().Replace(' ', '-'));   

            name = type.Name;


            //creating the ports
            if(nodeInfo.HasParent)
                CreateInputPort();
            else
            {
                inputContainer.RemoveFromHierarchy();
                this.Q(className: "input-title").RemoveFromHierarchy();
            }

            if (nodeInfo.HasChild)
                CreateOutputPort(nodeInfo);
            else
            {
                outputContainer.RemoveFromHierarchy();
                this.Q(className: "title-output").RemoveFromHierarchy();
            }

            //getting any properties of the node
            fields = new();
            foreach (FieldInfo property in type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                if(property.GetCustomAttribute<ShowFieldAttribute>() is ShowFieldAttribute)
                {
                    fields.Add(GetField(property.Name));
                }
            }

            if(fields.Count > 0)
            {
                nodeLabel = new(type.Name);
                nodeLabel.AddMarginTopClass();
                nodeLabel.AddToClassList("node-label");
            }
        }

        void CreateInputPort()
        {
            InputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(PortTypes.FlowPort));
            InputPort.portName = "";
            InputPort.tooltip = "The Parent of this node";
            inputContainer.Add(InputPort);
        }

        void CreateOutputPort(NodeInfoAttribute nodeInfo)
        {
            Port.Capacity capacity = nodeInfo.HasMultipleChildren ? Port.Capacity.Multi : Port.Capacity.Single;

            OutputPort = InstantiatePort(Orientation.Vertical, Direction.Output, capacity, typeof(PortTypes.FlowPort));
            OutputPort.portName = "";
            OutputPort.tooltip = nodeInfo.HasMultipleChildren ? "The Children of this node" : "The Child of this node";
            outputContainer.Add(OutputPort);
            Label outputPortName = outputContainer.Q<Label>(name: "Port-Name");
            outputPortName.text = nodeInfo.HasMultipleChildren ? "Children" : "Child";
        }

        public override void OnSelected()
        {
            if(fields.Count == 0)
                return;

            window.AddNodeFields(Node, nodeLabel, fields);
            
            base.OnSelected();
        }

        public override void OnUnselected()
        {
            if(fields.Count == 0)
                return;

            window.RemoveNodeFields(Node, nodeLabel, fields);

            base.OnUnselected();
        }

        PropertyField GetField(string name)
        {
            if(serializedNode == null)
                GetSerializedProperty();
            
            SerializedProperty serializedProperty = serializedNode.FindPropertyRelative(name);
            PropertyField field = new(serializedProperty);
            field.AddMarginTopClass();
            field.AddToClassList("node-property");
            return field;
        }

        void GetSerializedProperty()
        {
            SerializedProperty nodes = serializedGraph.FindProperty("nodes");
            if(nodes.isArray)
            {
                int size = nodes.arraySize;
                for (int i = 0; i < size; i++)
                {
                    var element = nodes.GetArrayElementAtIndex(i);
                    var elementID = element.FindPropertyRelative("id");
                    if(elementID.stringValue == Node.ID)
                    {
                        serializedNode = element;
                        return;
                    }
                }
            }
        }

        public void SetPosition() => Node.SetPosition(GetPosition());
    }
}
