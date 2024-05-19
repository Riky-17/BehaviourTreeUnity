using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace BehaviourTrees.Editor
{
    [CustomEditor(typeof(BehaviourTreeGraph))]
    public class BT_Editor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(GUILayout.Button("Open Graph"))
                BT_Window.OpenWindow((BehaviourTreeGraph)target);
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID)
        {
            Object asset = EditorUtility.InstanceIDToObject(instanceID);
            if(asset.GetType() == typeof(BehaviourTreeGraph))
            {
                BT_Window.OpenWindow((BehaviourTreeGraph)asset);
                return true;
            }

            return false;
        }
    }
}
