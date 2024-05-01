using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BehaviourTree.Editor
{
    [CustomEditor(typeof(MonoBehaviourTree), true, isFallback = false)]
    public class BehaviourTreeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(GUILayout.Button("Open Graph"))
                BehaviourTreeWindow.OpenWindow((MonoBehaviourTree)target);
        }
    }
}
