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
        MonoBehaviourTree behaviourTree;
        public BT_Window Window {get; private set;}
        BT_WindowSearch windowSearch;
        public List<BT_EditorNode> GraphNodes {get; private set;}
        public Dictionary<string, BT_EditorNode> GraphNodesDictionary {get; private set;}

        public BT_GraphView(SerializedObject serializedObject, BT_Window window)
        {
            this.serializedObject = serializedObject;
            behaviourTree = (MonoBehaviourTree)serializedObject.targetObject;
            Window = window;
            GraphNodes = new();
            GraphNodesDictionary = new();

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

        public void Add(BehaviourTreeNode node)
        {
            Undo.RecordObject(serializedObject.targetObject, "added Node");
            behaviourTree.Children.Add(node);
            serializedObject.Update();
            AddNodeToGraph(node);
        }

        void AddNodeToGraph(BehaviourTreeNode node)
        {
            BT_EditorNode editorNode = new();
            editorNode.SetPosition(node.Position);
            AddElement(editorNode);
            GraphNodes.Add(editorNode);
            GraphNodesDictionary.Add(node.Id, editorNode);
        }

        private void ShowWindowSearch(NodeCreationContext obj)
        {
            windowSearch.target = (VisualElement)focusController.focusedElement;
            SearchWindow.Open(new SearchWindowContext(obj.screenMousePosition), windowSearch);
        }
    }
}
