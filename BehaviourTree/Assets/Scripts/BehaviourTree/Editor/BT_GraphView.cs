using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree.Editor
{
    public class BT_GraphView : GraphView
    {
        SerializedObject serializedObject;
        MonoBehaviourTree behaviourTree;
        public BT_Window Window {get; private set;}
        BT_WindowSearch windowSearch;
        public List<BT_EditorNode> GraphNodes {get; private set;}
        public Dictionary<string, BT_EditorNode> GraphNodesDictionary {get; private set;}

        public BT_GraphView(SerializedObject serializedObject, BT_Window window)
        {
            this.serializedObject = serializedObject;
            behaviourTree = (MonoBehaviourTree)serializedObject.targetObject;
            Window = window;
            GraphNodes = new();
            GraphNodesDictionary = new();

            windowSearch = ScriptableObject.CreateInstance<BT_WindowSearch>();
            windowSearch.graphView = this;
            nodeCreationRequest = ShowWindowSearch;

            StyleSheet sheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/BehaviourTree/Editor/USS/BT_GraphGrid.uss");
            styleSheets.Add(sheet);

            GridBackground background = new()
            {
                name = "Grid"
            };
            Add(background); 
            background.SendToBack();

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());

            LoadNodes();

            graphViewChanged += OnGraphViewChanged;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.movedElements != null)
            {
                Undo.RecordObject(serializedObject.targetObject, "Moved Nodes");
                foreach (BT_EditorNode node in graphViewChange.movedElements.OfType<BT_EditorNode>())
                    node.SetPosition();
            }

            if (graphViewChange.elementsToRemove != null)
            {
                Undo.RecordObject(serializedObject.targetObject, "Removed Nodes");
                List<BT_EditorNode> nodes = graphViewChange.elementsToRemove.OfType<BT_EditorNode>().ToList();
                for (int i = nodes.Count - 1; i >= 0; i--)
                    RemoveNode(nodes[i]);
            }

            return graphViewChange;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> allPorts = new();
            List<Port> ports = new();

            foreach (BT_EditorNode node in GraphNodes)
                allPorts.AddRange(node.Ports);

            foreach (Port port in allPorts)
            {
                if(port == startPort)
                    continue;
                if(port.node == startPort.node)
                    continue;
                if(port.direction == startPort.direction)
                    continue;
                if(port.portType == startPort.portType)
                    ports.Add(port);
            }

            return ports;
        }

        private void RemoveNode(BT_EditorNode node)
        {
            behaviourTree.Children.Remove(node.Node);
            GraphNodes.Remove(node);
            GraphNodesDictionary.Remove(node.Node.Id);
            serializedObject.Update();
        }

        private void LoadNodes()
        {
            foreach (BehaviourTreeNode node in behaviourTree.Children)
                AddNodeToGraph(node);
        }

        public void Add(BehaviourTreeNode node)
        {
            Undo.RecordObject(serializedObject.targetObject, "added Node");
            behaviourTree.Children.Add(node);
            serializedObject.Update();
            AddNodeToGraph(node);
        }

        void AddNodeToGraph(BehaviourTreeNode node)
        {
            BT_EditorNode editorNode = new(node);
            editorNode.SetPosition(node.Position);
            AddElement(editorNode);
            GraphNodes.Add(editorNode);
            GraphNodesDictionary.Add(node.Id, editorNode);
        }

        private void ShowWindowSearch(NodeCreationContext obj)
        {
            windowSearch.target = (VisualElement)focusController.focusedElement;
            SearchWindow.Open(new SearchWindowContext(obj.screenMousePosition), windowSearch);
        }

        public void RefreshGraph()
        {
            foreach (BT_EditorNode editorNode in GraphNodes)
            {
                editorNode.SetPosition(editorNode.Node.Position);
                RemoveElement(editorNode);
            }

            GraphNodes.Clear();
            GraphNodesDictionary.Clear();
            LoadNodes();

        }
    }
}
