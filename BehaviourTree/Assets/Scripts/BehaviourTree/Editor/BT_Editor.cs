using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BehaviourTree.Editor
{
    [CustomEditor(typeof(MonoBehaviourTree), true)]
    public class BT_Editor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(GUILayout.Button("Open Graph"))
                BT_Window.OpenWindow((MonoBehaviourTree)target);
        }
    }
}