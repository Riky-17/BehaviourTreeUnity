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
        List<BehaviourTreeNode> selectedNodes;

        const string MARGIN_TOP = "margin-top";

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
            selectedNodes = new();
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
            headerGraphLabel.AddMarginTopClass(); 
            headerGraphLabel.AddToClassList(headerClass); 
            windowProperties.Add(headerGraphLabel);

            //the graph property
            SerializedProperty graphProperty = serializedWindow.FindProperty(nameof(graph));
            PropertyField graphPropertyField = new(graphProperty);
            graphPropertyField.AddMarginTopClass();
            graphPropertyField.AddToClassList("graph-property");
            serializedWindow.Update();
            graphPropertyField.Bind(serializedWindow);
            windowProperties.Add(graphPropertyField);
            EventCallback<SerializedPropertyChangeEvent> eventCallback = OnGraphPropertyFieldChange;
            graphPropertyField.RegisterValueChangeCallback(eventCallback);

            //header label for node properties
            Label headerNodeLabel = new("Nodes Properties");
            headerNodeLabel.AddMarginTopClass();
            headerNodeLabel.AddToClassList(headerClass);
            windowProperties.Add(headerNodeLabel);
        }

        public void AddNodeFields(BehaviourTreeNode node, Label nodeLabel, List<PropertyField> fields)
        {
            // string fieldClass = "node-property";
            if(selectedNodes.Contains(node))
                return;
            
            windowProperties.Add(nodeLabel);
            selectedNodes.Add(node);

            foreach (PropertyField field in fields)
            {
                windowProperties.Add(field);
                serializedGraph.Update();
                field.Bind(serializedGraph);
            }
        }

        public void RemoveNodeFields(BehaviourTreeNode node, Label nodeLabel, List<PropertyField> fields)
        {
            if(!selectedNodes.Contains(node))
                return;
            
            nodeLabel.RemoveFromHierarchy();
            selectedNodes.Remove(node);

            foreach (PropertyField field in fields)
            {
                field.RemoveFromHierarchy();
                serializedGraph.Update();
                field.Unbind();
            }
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
