using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTrees.Editor
{
    public class BT_WindowSearch : ScriptableObject, ISearchWindowProvider
    {
        public BT_GraphView graphView;
        public VisualElement target;

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> tree = new()
            {
                new SearchTreeGroupEntry(new GUIContent("Nodes"))
            };
            List<EntryData> entryData = new();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if(type.GetCustomAttributes().ToList() != null)
                    {
                        var attribute = type.GetCustomAttribute(typeof(NodeInfoAttribute));
                        if(attribute != null)
                        {
                            var node = Activator.CreateInstance(type);
                            SearchTreeEntry entry = new(new(type.Name))
                            {
                                level = 1,
                                userData = new EntryData(type.Name, node)
                            };
                            tree.Add(entry);
                            entryData.Add(new(type.Name, node));
                        }
                    }
                }
            }


            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var windowMousePosition = graphView.ChangeCoordinatesTo(graphView, context.screenMousePosition - graphView.Window.position.position);
            var graphMousePosition = graphView.contentViewContainer.WorldToLocal(windowMousePosition);

            EntryData data = (EntryData)SearchTreeEntry.userData;
            BehaviourTreeNode node = (BehaviourTreeNode)data.Data;
            node.SetPosition(new(graphMousePosition, new()));
            graphView.Add(node);
            
            return true;
        }
    }

    public struct EntryData
    {
        public string Title {get; private set;}
        public object Data {get; private set;}

        public EntryData(string title, object data)
        {
            Title = title;
            Data = data;
        }
    }
}
