using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BehaviourTree.Editor
{
    public class BehaviourTreeWindow : EditorWindow
    {
        MonoBehaviourTree behaviourTree;

        public static void OpenWindow(MonoBehaviourTree behaviourTree)
        {
            BehaviourTreeWindow[] bTWindows = Resources.FindObjectsOfTypeAll<BehaviourTreeWindow>();

            foreach (BehaviourTreeWindow bTWindow in bTWindows)
            {
                if(bTWindow.behaviourTree == behaviourTree)
                {
                    bTWindow.Focus();
                    return;
                }
            }

            BehaviourTreeWindow btWindow = CreateWindow<BehaviourTreeWindow>(typeof(BehaviourTreeWindow), typeof(SceneView));
            btWindow.titleContent = new GUIContent(behaviourTree.name);
        }
    }
}
