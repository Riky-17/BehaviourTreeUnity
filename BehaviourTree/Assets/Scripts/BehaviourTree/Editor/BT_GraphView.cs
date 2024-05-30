using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTrees.Editor
{
    public class BT_GraphView : GraphView
    {
        SerializedObject serializedGraph;
        BehaviourTreeGraph behaviourTreeGraph;
        BT_WindowSearch windowSearch;

        public BT_Window Window {get; private set;}
        public List<BT_EditorNode> GraphNodes {get; private set;}
        public Dictionary<string, BT_EditorNode> GraphNodesDictionary {get; private set;}
        public Dictionary<Edge, BehaviourTreeConnection> ConnectionsDictionary {get; private set;}

        public BT_GraphView(SerializedObject serializedGraph, BT_Window window)
        {
            this.serializedGraph = serializedGraph;
            behaviourTreeGraph = (BehaviourTreeGraph)serializedGraph.targetObject;
            Window = window;
            GraphNodes = new();
            GraphNodesDictionary = new();
            ConnectionsDictionary = new();
            

            windowSearch = ScriptableObject.CreateInstance<BT_WindowSearch>();
            windowSearch.graphView = this;
            nodeCreationRequest = ShowWindowSearch;

            GridBackground background = new()
            {
                name = "Grid"
            };
            Add(background); 
            background.SendToBack();

            SelectionDragger selectionDragger = new();

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());

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
                if(behaviourTreeGraph.AreNodesConnected(((BT_EditorNode)startPort.node).Node, ((BT_EditorNode)port.node).Node))
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
                behaviourTreeGraph.Connections.Remove(connection);
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
            BT_EditorNode editorNode = new(node);
            editorNode.SetPosition(node.Position);
            AddElement(editorNode);
            GraphNodes.Add(editorNode);
            GraphNodesDictionary.Add(node.ID, editorNode);
        }

        void CreateEdge(Edge edge)
        {
            BT_EditorNode inputNode = (BT_EditorNode)edge.input.node;
            BT_EditorNode outputNode = (BT_EditorNode)edge.output.node;

            BehaviourTreeConnection connection = new(new(inputNode.Node), new(outputNode.Node));
            behaviourTreeGraph.Connections.Add(connection);
            ConnectionsDictionary.Add(edge, connection);
            serializedGraph.Update();
        }

        void LoadConnections()
        {
            foreach (BehaviourTreeConnection connection in behaviourTreeGraph.Connections)
                DrawConnection(connection);
        }

        void DrawConnection(BehaviourTreeConnection connection)
        {
            BT_EditorNode inputNode = GetNode(connection.inputPort.NodeID);
            BT_EditorNode outputNode = GetNode(connection.outputPort.NodeID);
            if(inputNode == null || outputNode == null)
                return;
            Port inputPort = inputNode.InputPort;
            Port outputPort = outputNode.OutputPort;
            Edge edge = inputPort.ConnectTo(outputPort);
            ConnectionsDictionary.Add(edge, connection);
            AddElement(edge);
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
            foreach (BT_EditorNode editorNode in GraphNodes)
            {
                editorNode.SetPosition(editorNode.Node.Position);
                RemoveElement(editorNode);
            }

            foreach (Edge edge in ConnectionsDictionary.Keys)
                RemoveElement(edge);

            GraphNodes.Clear();
            GraphNodesDictionary.Clear();
            ConnectionsDictionary.Clear();
            LoadNodes();
            LoadConnections();
        }
    }
}
