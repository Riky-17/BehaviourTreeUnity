using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace BehaviourTrees.Editor
{
    public class BT_Window : EditorWindow
    {
        BehaviourTreeGraph graph;
        SerializedObject serializedObject;
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
            Undo.undoRedoPerformed += OnUndoRedoPerformed;

            if(graph != null)
                DrawGraph();
        }

        void OnDisable() => Undo.undoRedoPerformed -= OnUndoRedoPerformed;

        void Load(BehaviourTreeGraph graph)
        {
            this.graph = graph;
            DrawGraph();
        }

        void DrawGraph()
        {
            serializedObject = new(graph);
            graphView = new(serializedObject, this);
            rootVisualElement.Add(graphView);
        }

        private void OnUndoRedoPerformed() => graphView.RefreshGraph();
    }
}
