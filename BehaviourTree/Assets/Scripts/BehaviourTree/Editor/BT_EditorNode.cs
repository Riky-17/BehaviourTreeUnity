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
        readonly BT_Window window;
        SerializedProperty serializedNode;
        readonly SerializedObject serializedGraph;

        readonly Label nodeLabel;
        readonly List<PropertyField> fields;
        ListView dataNodeListView;

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
                //removing the divider element
                this.Q(className: "input-title").RemoveFromHierarchy();
            }

            if (nodeInfo.HasChild)
                CreateOutputPort(nodeInfo);
            else
            {
                outputContainer.RemoveFromHierarchy();
                //removing the divider element
                this.Q(className: "title-output").RemoveFromHierarchy();
            }
            
            if (Node is DataNode)
            {
                //creating the DataNode ListView
                SetDataNodeListView();
                
                nodeLabel = new(type.Name.AddSpaces());
                nodeLabel.AddToClassList("margin-top", "node-label");
            }
            else if (Node is Leaf)
            {
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
                    nodeLabel = new(type.Name.AddSpaces());
                    nodeLabel.AddToClassList("margin-top", "node-label");
                }
            }
        }

        void SetDataNodeListView()
        {
            DataNode dataNode = Node as DataNode;
            
            if(serializedNode == null)
                    GetSerializedProperty();

            List<DataNodeEntry> entries = dataNode.entries;

            dataNodeListView = new()
            {
                headerTitle ="Entries",
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                showFoldoutHeader = true,
                makeItem = MakeItem,
                bindItem = BindItem,
                unbindItem = UnbindItem,
                showAddRemoveFooter = true,
                bindingPath = serializedNode.FindPropertyRelative("entries").propertyPath
            };

            dataNodeListView.AddToClassList("margin-top");
            dataNodeListView.Bind(serializedGraph);

            VisualElement MakeItem()
            {
                BindableElement element = new();
                element.AddToClassList("data-node-element");

                VisualElement keyContainer = new();
                keyContainer.AddToClassList("row");
                
                keyContainer.Add(new Label("Key"));
                TextField key = new() { bindingPath = nameof(DataNodeEntry.key) };
                keyContainer.Add(key);
                element.Add(keyContainer);

                VisualElement valueTypeContainer = new();
                valueTypeContainer.AddToClassList("row");

                valueTypeContainer.Add(new Label("Type"));
                EnumField valueType = new() { bindingPath = nameof(DataNodeEntry.valueType) };
                valueTypeContainer.Add(valueType);
                element.Add(valueTypeContainer);

                VisualElement valueContainer = new();
                valueContainer.AddToClassList("row");

                valueContainer.Add(new Label("Value"));
                valueContainer.AddToClassList("value-container");
                element.Add(valueContainer);

                return element;
            }

            void BindItem(VisualElement element, int index)
            {
                var field = element as IBindable;

                AnyValue.ValueTypes valueType = entries[index].valueType;
                EnumField enumField = element.Q<EnumField>();
                enumField.RegisterCallback<ChangeEvent<Enum>, VisualElement>(ValueTypeChanged, element);
                enumField.value = valueType;

                var itemProp = (SerializedProperty)dataNodeListView.itemsSource[index];
                field.bindingPath = itemProp.propertyPath;
                element.Bind(serializedGraph);
            }
            
            void UnbindItem(VisualElement element, int index)
            {
                EnumField enumField = element.Q<EnumField>();
                enumField.UnregisterCallback<ChangeEvent<Enum>, VisualElement>(ValueTypeChanged);
                element.Unbind();
            }

            void ValueTypeChanged(ChangeEvent<Enum> evt, VisualElement element)
            {
                if(evt.newValue is not AnyValue.ValueTypes)
                    return;

                VisualElement valueContainer = element.Q(className: "value-container");

                element.Unbind();
                valueContainer.Clear();
                valueContainer.Add(new Label("Value"));
                switch ((AnyValue.ValueTypes)evt.newValue)
                {
                    case AnyValue.ValueTypes.Int:
                        valueContainer.Add(new IntegerField() { bindingPath = $"{nameof(DataNodeEntry.value)}.{nameof(AnyValue.intValue)}" });
                        break;
                    case AnyValue.ValueTypes.Float:
                        valueContainer.Add(new FloatField() { bindingPath = $"{nameof(DataNodeEntry.value)}.{nameof(AnyValue.floatValue)}" });
                        break;
                    case AnyValue.ValueTypes.Bool:
                        valueContainer.Add(new Toggle() { bindingPath = $"{nameof(DataNodeEntry.value)}.{nameof(AnyValue.boolValue)}" });
                        break;
                    case AnyValue.ValueTypes.String:
                        valueContainer.Add(new TextField() { bindingPath = $"{nameof(DataNodeEntry.value)}.{nameof(AnyValue.stringValue)}" });
                        break;
                    case AnyValue.ValueTypes.Vector3:
                        valueContainer.Add(new Vector3Field() { bindingPath = $"{nameof(DataNodeEntry.value)}.{nameof(AnyValue.vector3Value)}" });
                        break;
                    case AnyValue.ValueTypes.GameObject:
                        valueContainer.Add(new ObjectField() { bindingPath = $"{nameof(DataNodeEntry.value)}.{nameof(AnyValue.gameObjectValue)}", objectType = typeof(GameObject) });
                        break;
                    case AnyValue.ValueTypes.Transform:
                        valueContainer.Add(new ObjectField() { bindingPath = $"{nameof(DataNodeEntry.value)}.{nameof(AnyValue.transformValue)}", objectType = typeof(Transform) });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                element.Bind(serializedGraph);
                serializedGraph.Update();
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
            if (fields != null && fields.Count != 0)
            {
                window.AddNodeFields(Node, nodeLabel, fields);
                base.OnSelected();
            }
            else if(Node is DataNode)
            {
                window.AddDataNodeList(Node, nodeLabel, dataNodeListView);
                base.OnSelected();
            }
        }

        public override void OnUnselected()
        {
            if (fields != null && fields.Count != 0)
            {
                window.RemoveNodeFields(Node, nodeLabel, fields);
                base.OnUnselected();
            }
            else if(Node is DataNode)
            {
                window.RemoveDataNodeList(Node, nodeLabel, dataNodeListView);
                base.OnUnselected();
            }
        }

        PropertyField GetField(string name)
        {
            if(serializedNode == null)
                GetSerializedProperty();
            
            SerializedProperty serializedProperty = serializedNode.FindPropertyRelative(name);
            PropertyField field = new(serializedProperty);
            field.AddToClassList("margin-top");
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
