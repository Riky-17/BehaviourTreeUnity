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
            StyleSheet sheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/BehaviourTree/Editor/USS/BT_GraphGrid.uss");
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

        void DrawGraph()
        {
            serializedGraph = new(graph);
            graphView = new(serializedGraph, this);
            VisualElement graphContainer = new();
            graphContainer.AddToClassList("graph-container");
            rootVisualElement.Add(graphContainer);
            graphContainer.Add(graphView);
        }

        void AddPropertyWindow()
        {
            serializedWindow = new(this);
            VisualElement windowProperties = new();
            windowProperties.AddToClassList("window-properties");
            rootVisualElement.Add(windowProperties);
            SerializedProperty graphProperty = serializedWindow.FindProperty(nameof(graph));
            PropertyField graphPropertyField = new(graphProperty);
            graphPropertyField.AddToClassList("graph-property");
            graphPropertyField.Bind(serializedWindow);
            windowProperties.Add(graphPropertyField);
            EventCallback<SerializedPropertyChangeEvent> eventCallback = OnGraphPropertyFieldChange;
            graphPropertyField.RegisterValueChangeCallback(eventCallback);
        }

        void OnGraphPropertyFieldChange(SerializedPropertyChangeEvent evt)
        {
            serializedWindow.Update();
            titleContent = new(graph.name);
            rootVisualElement.RemoveAt(1);
            DrawGraph();
        }

        void OnUndoRedoPerformed() => graphView.RefreshGraph();
    }
}
