using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

namespace BehaviourTrees.Editor
{
    public class BT_Window : EditorWindow
    {
        [SerializeField] BehaviourTreeGraph graph;
        SerializedObject serializedGraph;
        SerializedObject serializedWindow;
        
        BT_GraphView graphView;

        VisualElement windowProperties;
        Label noSelectedNodesLabel;
        public List<BT_EditorNode> SelectedNodes { get; private set; }

        const string MARGIN_TOP = "margin-top";
        const string CENTERED = "centered";

        public static void OpenWindow(BehaviourTreeGraph graph)
        {
            BT_Window[] bTWindows = Resources.FindObjectsOfTypeAll<BT_Window>();

            foreach (BT_Window bTWindow in bTWindows)
            {
                if(bTWindow.graph == graph)
                {
                    bTWindow.Focus();
                    return;
                }
            }

            BT_Window btWindow = CreateWindow<BT_Window>(typeof(BT_Window), typeof(SceneView));
            btWindow.titleContent = new GUIContent(graph.name);
            btWindow.Load(graph);
        }

        void OnEnable()
        {
            StyleSheet sheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/BehaviourTree/Editor/Style/BT_GraphStyle.uss");
            noSelectedNodesLabel = new("No nodes have been selected");
            noSelectedNodesLabel.AddToClassList(MARGIN_TOP, CENTERED);
            SelectedNodes = new();
            rootVisualElement.styleSheets.Add(sheet);

            Undo.undoRedoPerformed += OnUndoRedoPerformed;

            if(graph != null)
                PaintWindow();
        }

        void OnDisable() => Undo.undoRedoPerformed -= OnUndoRedoPerformed;

        void Load(BehaviourTreeGraph graph)
        {
            this.graph = graph;
            
            PaintWindow();
        }
        
        void PaintWindow()
        {
            AddPropertyWindow();
            DrawGraph();
        }

        void AddPropertyWindow()
        {

            string headerClass = "header-label";
            // serialize window object
            serializedWindow = new(this);

            //the side of the window where to put properties
            windowProperties = new();
            windowProperties.AddToClassList("window-properties");
            rootVisualElement.Add(windowProperties);

            //header label for graph properties
            Label headerGraphLabel = new("Graph Properties");
            headerGraphLabel.AddToClassList(MARGIN_TOP, headerClass, CENTERED); 
            windowProperties.Add(headerGraphLabel);

            //the graph property
            SerializedProperty graphProperty = serializedWindow.FindProperty(nameof(graph));
            PropertyField graphPropertyField = new(graphProperty);
            graphPropertyField.AddToClassList(MARGIN_TOP);
            serializedWindow.Update();
            graphPropertyField.Bind(serializedWindow);
            windowProperties.Add(graphPropertyField);
            EventCallback<SerializedPropertyChangeEvent> eventCallback = OnGraphPropertyFieldChange;
            graphPropertyField.RegisterValueChangeCallback(eventCallback);

            //header label for node properties
            Label headerNodeLabel = new("Nodes Properties");
            headerNodeLabel.AddToClassList(MARGIN_TOP, headerClass, CENTERED);
            windowProperties.Add(headerNodeLabel);
            windowProperties.Add(noSelectedNodesLabel);
        }

        public void AddNodeFields(BT_EditorNode node, Label nodeLabel, List<PropertyField> fields)
        {
            if(SelectedNodes.Contains(node))
                return;
            
            windowProperties.Add(nodeLabel);
            SelectedNodes.Add(node);

            foreach (VisualElement field in fields)
            {
                windowProperties.Add(field);
                serializedGraph.Update();
                field.Bind(serializedGraph);
            }

            noSelectedNodesLabel.RemoveFromHierarchy();
        }

        public void RemoveNodeFields(BT_EditorNode node, Label nodeLabel, List<PropertyField> fields)
        {
            if(!SelectedNodes.Contains(node))
                return;
            
            nodeLabel.RemoveFromHierarchy();
            SelectedNodes.Remove(node);

            foreach (VisualElement field in fields)
            {
                field.RemoveFromHierarchy();
                serializedGraph.Update();
                field.Unbind();
            }

            if(SelectedNodes.Count == 0)
                windowProperties.Add(noSelectedNodesLabel);
        }

        public void AddDataNodeList(BT_EditorNode node, Label nodeLabel, VisualElement element)
        {
            if(SelectedNodes.Contains(node))
                return;

            windowProperties.Add(nodeLabel);
            SelectedNodes.Add(node);

            windowProperties.Add(element);
            serializedGraph.Update();

            noSelectedNodesLabel.RemoveFromHierarchy();
        }

        public void RemoveDataNodeList(BT_EditorNode node, Label nodeLabel, VisualElement element)
        {
            if(!SelectedNodes.Contains(node))
                return;

            nodeLabel.RemoveFromHierarchy();
            SelectedNodes.Remove(node);

            element.RemoveFromHierarchy();
            serializedGraph.Update();

            if(SelectedNodes.Count == 0)
                windowProperties.Add(noSelectedNodesLabel);
        }

        void DrawGraph()
        {
            serializedGraph = new(graph);
            VisualElement graphContainer = new();
            graphContainer.AddToClassList("graph-container");
            rootVisualElement.Add(graphContainer);
            graphView = new(serializedGraph, this);
            graphView.graphViewChanged += OnGraphViewChanged;
            graphContainer.Add(graphView);
        }

        void OnGraphPropertyFieldChange(SerializedPropertyChangeEvent evt)
        {
            serializedWindow.Update();
            titleContent = new(graph.name);
            rootVisualElement.RemoveAt(1);
            DrawGraph();
        }

        GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            EditorUtility.SetDirty(graph);
            return graphViewChange;
        }

        void OnUndoRedoPerformed() => graphView.RefreshGraph();
    }
}
