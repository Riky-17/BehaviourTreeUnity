using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BehaviourTree.Editor
{
    public class BT_Window : EditorWindow
    {
        MonoBehaviourTree behaviourTree;
        SerializedObject serializedObject;
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

        void Load(MonoBehaviourTree behaviourTree)
        {
            this.behaviourTree = behaviourTree;
            DrawGraph();
        }

        void DrawGraph()
        {
            serializedObject = new(behaviourTree);
            graphView = new(serializedObject, this);
            rootVisualElement.Add(graphView);
        }
    }
}
