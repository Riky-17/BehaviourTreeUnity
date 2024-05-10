using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System;

namespace BehaviourTree.Editor
{
    public class BT_Window : EditorWindow
    {
        MonoBehaviourTree behaviourTree;
        SerializedObject serializedObject;
        BT_Window window;
        BT_GraphView graphView;

        public static void OpenWindow(MonoBehaviourTree behaviourTree)
        {
            BT_Window[] bTWindows = Resources.FindObjectsOfTypeAll<BT_Window>();

            foreach (BT_Window bTWindow in bTWindows)
            {
                if(bTWindow.behaviourTree.GetType() == behaviourTree.GetType())
                {
                    bTWindow.Focus();
                    return;
                }
            }

            BT_Window btWindow = CreateWindow<BT_Window>(typeof(BT_Window), typeof(SceneView));
            btWindow.titleContent = new GUIContent(behaviourTree.GetType().Name);
            btWindow.Load(behaviourTree);
        }

        void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoRedoPerformed;

            if(behaviourTree != null)
                DrawGraph();
        }

        void OnDisable() => Undo.undoRedoPerformed -= OnUndoRedoPerformed;

        void OnGUI()
        {
            if(behaviourTree != null)
            {
                if(EditorUtility.IsDirty(behaviourTree))
                    hasUnsavedChanges = true;
                else
                    hasUnsavedChanges = false;
            }
        }

        void Load(MonoBehaviourTree behaviourTree)
        {
            this.behaviourTree = behaviourTree;
            DrawGraph();
        }

        void DrawGraph()
        {
            serializedObject = new(behaviourTree);
            graphView = new(serializedObject, this);
            graphView.graphViewChanged += OnGraphViewChanged;
            rootVisualElement.Add(graphView);
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            EditorUtility.SetDirty(behaviourTree);
            return graphViewChange;
        }

        private void OnUndoRedoPerformed()
        {
            rootVisualElement.Remove(graphView);
            DrawGraph();
        }
    }
}
