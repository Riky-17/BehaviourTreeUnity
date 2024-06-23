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
        static int testClick = 0;

        readonly Label nodeLabel;
        readonly List<PropertyField> fields;
        readonly ListView dataNodeList;

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

            //getting any properties of the node
            fields = new();
            
            if (Node is DataNode dataNode)
            {
                List<DataNodeEntryData> entries = dataNode.entries;

                if(serializedNode == null)
                    GetSerializedProperty();

                //const int itemHeight = 100;


                dataNodeList = new()
                {
                    headerTitle ="Entries",
                    virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                    showFoldoutHeader = true,
                    makeItem = MakeItem,
                    bindItem = BindItem,
                    unbindItem = UnbindItem,
                    //fixedItemHeight = itemHeight,
                    showAddRemoveFooter = true,
                    bindingPath = serializedNode.FindPropertyRelative("entries").propertyPath
                };

                dataNodeList.Bind(serializedGraph);

                VisualElement MakeItem()
                {
                    //Debug.Log("Hello");
                    BindableElement element = new();
                    
                    TextField keyField = new() { bindingPath = nameof(DataNodeEntryData.key) };

                    element.Add(keyField);

                    EnumField valueType = new() { bindingPath = nameof(DataNodeEntryData.valueType) };

                    VisualElement valueContainer = new();
                    valueContainer.AddToClassList("value-container");

                    // Debug.Log("Registering Event");
                    //valueType.RegisterCallback<ChangeEvent<Enum>, VisualElement>(ValueTypeChanged, valueContainer);

                    element.Add(valueType);

                    element.Add(valueContainer);
                    return element;
                }

                void BindItem(VisualElement element, int index)
                {
                    //Debug.Log("Binding " + index);
                    var field = element as IBindable;


                    AnyValue.ValueTypes valueType = entries[index].valueType;
                    EnumField enumField = element.Q<EnumField>();
                    VisualElement valueContainer = element.Q(className: "value-container");
                    enumField.RegisterCallback<ChangeEvent<Enum>, VisualElement>(ValueTypeChanged, element);
                    enumField.value = valueType;
                    //enumField.SetValueWithoutNotify(valueType);

                    //Toggle toggle = element.Q<Toggle>();
                    //Debug.Log(toggle);

                    var itemProp = (SerializedProperty)dataNodeList.itemsSource[index];
                    field.bindingPath = itemProp.propertyPath;
                    element.Bind(serializedGraph);
                }
                
                void UnbindItem(VisualElement element, int index)
                {
                    EnumField enumField = element.Q<EnumField>();
                    enumField.UnregisterCallback<ChangeEvent<Enum>, VisualElement>(ValueTypeChanged);
                }

                nodeLabel = new(type.Name.AddSpaces());
                nodeLabel.AddToClassList("margin-top", "node-label");
            }
            else if (Node is Leaf)
            {
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

        void ValueTypeChanged(ChangeEvent<Enum> evt, VisualElement element)
        {
            Debug.Log(evt.newValue);
            if(evt.newValue is not AnyValue.ValueTypes)
                return;
            
            var field = element as IBindable;
            Debug.Log(field.bindingPath);

            VisualElement valueContainer = element.Q(className: "value-container");

            element.Unbind();
            valueContainer.Clear();
            switch ((AnyValue.ValueTypes)evt.newValue)
            {
                case AnyValue.ValueTypes.Int:
                    IntegerField integerField = new () { bindingPath = nameof(DataNodeEntryData.value) + "." + nameof(AnyValue.intValue) };
                    valueContainer.Add(integerField);
                    break;
                case AnyValue.ValueTypes.Float:
                    FloatField floatField = new () { bindingPath = nameof(DataNodeEntryData.value) + "." + nameof(AnyValue.floatValue) };
                    valueContainer.Add(floatField);
                    break;
                case AnyValue.ValueTypes.Bool:
                    Toggle test = new() { bindingPath = nameof(DataNodeEntryData.value) + "." + nameof(AnyValue.boolValue) };
                    valueContainer.Add(test);
                    break;
                case AnyValue.ValueTypes.String:
                    TextField textField = new () { bindingPath = nameof(DataNodeEntryData.value) + "." + nameof(AnyValue.stringValue) };
                    valueContainer.Add(textField);
                    break;
                case AnyValue.ValueTypes.Vector3:
                    Vector3Field vector3Field = new () { bindingPath = nameof(DataNodeEntryData.value) + "." + nameof(AnyValue.vector3Value) };
                    valueContainer.Add(vector3Field);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            element.Bind(serializedGraph);
            serializedGraph.Update();
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
            if (fields.Count != 0)
            {
                window.AddNodeFields(Node, nodeLabel, fields);
                base.OnSelected();
            }
            else if(Node is DataNode)
            {
                testClick++;
                window.AddDataNodeList(Node, nodeLabel, dataNodeList);
                base.OnSelected();
            }
        }

        public override void OnUnselected()
        {
            if (fields.Count != 0)
            {
                window.RemoveNodeFields(Node, nodeLabel, fields);
                base.OnUnselected();
            }
            else if(Node is DataNode)
            {
                window.RemoveDataNodeList(Node, nodeLabel, dataNodeList);
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
