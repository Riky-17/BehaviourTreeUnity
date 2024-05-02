using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree.Editor
{
    public class BT_GraphView : GraphView
    {
        SerializedObject serializedObject;
        BT_WindowSearch windowSearch;

        public BT_GraphView(SerializedObject serializedObject)
        {
            this.serializedObject = serializedObject;

            windowSearch = ScriptableObject.CreateInstance<BT_WindowSearch>();
            windowSearch.graphView = this;
            nodeCreationRequest = ShowWindowSearch;

            StyleSheet sheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/BehaviourTree/Editor/USS/BT_GraphGrid.uss");
            styleSheets.Add(sheet);

            GridBackground background = new()
            {
                name = "Grid"
            };
            Add(background); 

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
        }

        private void ShowWindowSearch(NodeCreationContext obj)
        {
            windowSearch.target = (VisualElement)focusController.focusedElement;
            SearchWindow.Open(new SearchWindowContext(obj.screenMousePosition), windowSearch);
        }
    }
}
