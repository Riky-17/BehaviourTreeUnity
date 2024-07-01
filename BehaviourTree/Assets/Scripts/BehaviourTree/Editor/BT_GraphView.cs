using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTrees.Editor
{
    public class BT_GraphView : GraphView
    {
        SerializedObject serializedGraph;
        BehaviourTreeGraph behaviourTreeGraph;
        BT_SearchWindow windowSearch;

        public BT_Window Window {get; private set;}

        public List<BT_EditorNode> GraphNodes {get; private set;}
        public Dictionary<string, BT_EditorNode> GraphNodesDictionary {get; private set;}
        public Dictionary<Edge, BehaviourTreeConnection> ConnectionsDictionary {get; private set;}
        List<BT_EditorNode> selectedNodes;

        public BT_GraphView(SerializedObject serializedGraph, BT_Window window)
        {
            this.serializedGraph = serializedGraph;
            behaviourTreeGraph = (BehaviourTreeGraph)serializedGraph.targetObject;
            Window = window;
            GraphNodes = new();
            GraphNodesDictionary = new();
            ConnectionsDictionary = new();
            

            windowSearch = ScriptableObject.CreateInstance<BT_SearchWindow>();
            windowSearch.graphView = this;
            nodeCreationRequest = ShowWindowSearch;

            GridBackground background = new() { name = "Grid" };
            Add(background); 
            background.SendToBack();

            SelectionDragger selectionDragger = new();

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
            
            
            AddNodeToGraph(behaviourTreeGraph.Root);
            LoadNodes();
            LoadConnections();
            
            graphViewChanged += OnGraphViewChanged;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.movedElements != null)
            {
                Undo.RecordObject(serializedGraph.targetObject, "Moved Nodes");
                foreach (BT_EditorNode node in graphViewChange.movedElements.OfType<BT_EditorNode>())
                    node.SetPosition();
            }

            if (graphViewChange.elementsToRemove != null)
            {
                Undo.RecordObject(serializedGraph.targetObject, "Removed Elements");
                List<BT_EditorNode> nodes = graphViewChange.elementsToRemove.OfType<BT_EditorNode>().ToList();
                for (int i = nodes.Count - 1; i >= 0; i--)
                {
                    RemoveNode(nodes[i]);
                }

                List<Edge> edges = graphViewChange.elementsToRemove.OfType<Edge>().ToList();
                for (int i = edges.Count - 1; i >= 0 ; i--)
                    RemoveConnection(edges[i]);
            }

            if(graphViewChange.edgesToCreate != null)
            {
                Undo.RecordObject(serializedGraph.targetObject, "Connection Added");
                foreach(Edge edge in graphViewChange.edgesToCreate)
                    CreateEdge(edge);
            }

            return graphViewChange;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> allPorts = new();
            List<Port> ports = new();

            foreach (BT_EditorNode node in GraphNodes)
            {
                if(node.InputPort != null)
                    allPorts.Add(node.InputPort);
                if(node.OutputPort != null)
                    allPorts.Add(node.OutputPort);
            }

            foreach (Port port in allPorts)
            {
                if(port == startPort)
                    continue;
                if(port.node == startPort.node)
                    continue;
                if(port.direction == startPort.direction)
                    continue;
                if(startPort.direction == Direction.Output && behaviourTreeGraph.AreNodesConnected(((BT_EditorNode)startPort.node).Node, ((BT_EditorNode)port.node).Node))
                    continue;
                if (port.portType == startPort.portType)
                    ports.Add(port);
            }

            return ports;
        }

        private void RemoveNode(BT_EditorNode node)
        {
            behaviourTreeGraph.Nodes.Remove(node.Node);
            GraphNodes.Remove(node);
            GraphNodesDictionary.Remove(node.Node.ID);
            serializedGraph.Update();
        }

        void RemoveConnection(Edge edge)
        {
            if(ConnectionsDictionary.TryGetValue(edge, out BehaviourTreeConnection connection))
            {
                if(connection.ChildrenCount <= 1)
                    behaviourTreeGraph.Connections.Remove(connection);
                else
                {
                    BT_EditorNode childNode = (BT_EditorNode)edge.input.node;
                    connection.RemoveChild(childNode.Node);
                }

                ConnectionsDictionary.Remove(edge);
                serializedGraph.Update();
            }
        }

        private void LoadNodes()
        {
            foreach (BehaviourTreeNode node in behaviourTreeGraph.Nodes)
                AddNodeToGraph(node);
        }

        public void Add(BehaviourTreeNode node)
        {
            Undo.RecordObject(serializedGraph.targetObject, "added Node");
            behaviourTreeGraph.Nodes.Add(node);
            serializedGraph.Update();
            AddNodeToGraph(node);
        }

        void AddNodeToGraph(BehaviourTreeNode node)
        {
            BT_EditorNode editorNode = new(node, serializedGraph, Window);
            editorNode.SetPosition(node.Position);
            AddElement(editorNode);
            GraphNodes.Add(editorNode);
            GraphNodesDictionary.Add(node.ID, editorNode);
            Bind();

            if(selectedNodes == null || selectedNodes.Count == 0)
                return;
            
            foreach (BT_EditorNode selectedNode in selectedNodes)
            {
                if(selectedNode.Node == node)
                {
                    AddToSelection(editorNode);
                    selectedNodes.Remove(selectedNode);
                    return;
                }
            }
        }

        void CreateEdge(Edge edge)
        {
            BT_EditorNode parentNode = (BT_EditorNode)edge.output.node;
            BT_EditorNode childNode = (BT_EditorNode)edge.input.node;

            foreach (BehaviourTreeConnection connection in behaviourTreeGraph.Connections)
            {
                if(connection.parentNode == parentNode.Node)
                {
                    connection.AddChild(childNode.Node);
                    ConnectionsDictionary.Add(edge, connection);
                    serializedGraph.Update();
                    return;
                }
            }

            BehaviourTreeConnection nodesConnection = new(parentNode.Node, childNode.Node);
            behaviourTreeGraph.Connections.Add(nodesConnection);
            ConnectionsDictionary.Add(edge, nodesConnection);
            serializedGraph.Update();
        }

        void LoadConnections()
        {
            foreach (BehaviourTreeConnection connection in behaviourTreeGraph.Connections)
                DrawConnection(connection);
        }

        void DrawConnection(BehaviourTreeConnection connection)
        {
            BT_EditorNode parentNode = GetNode(connection.parentNode.ID);
            List<BehaviourTreeNode> childrenNodes = connection.childrenNodes;

            if(childrenNodes == null || childrenNodes.Count == 0 || parentNode == null)
            {
                behaviourTreeGraph.Connections.Remove(connection);
                return;
            }

            foreach (BehaviourTreeNode child in childrenNodes)
            {
                BT_EditorNode childNode = GetNode(child.ID);
                Port inputPort = childNode.InputPort;
                Port outputPort = parentNode.OutputPort;
                Edge edge = inputPort.ConnectTo(outputPort);
                ConnectionsDictionary.Add(edge, connection);
                AddElement(edge);
            }
        }

        private BT_EditorNode GetNode(string nodeID)
        {
            GraphNodesDictionary.TryGetValue(nodeID, out BT_EditorNode node);
            return node;
        }

        private void ShowWindowSearch(NodeCreationContext obj)
        {
            windowSearch.target = (VisualElement)focusController.focusedElement;
            SearchWindow.Open(new SearchWindowContext(obj.screenMousePosition), windowSearch);
        }

        public void RefreshGraph()
        {
            selectedNodes = new(Window.SelectedNodes);

            foreach (BT_EditorNode editorNode in GraphNodes)
            {
                editorNode.SetPosition(editorNode.Node.Position);
                editorNode.OnUnselected();
                RemoveElement(editorNode);
            }

            foreach (Edge edge in ConnectionsDictionary.Keys)
                RemoveElement(edge);

            GraphNodes.Clear();
            GraphNodesDictionary.Clear();
            ConnectionsDictionary.Clear();
            AddNodeToGraph(behaviourTreeGraph.Root);
            LoadNodes();
            LoadConnections();
        }

        void Bind()
        {
            serializedGraph.Update();
            this.Bind(serializedGraph);
        }
    }
}
