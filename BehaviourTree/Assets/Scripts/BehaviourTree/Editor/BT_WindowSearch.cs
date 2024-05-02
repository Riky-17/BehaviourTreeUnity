using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree.Editor
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
            // List<string> nodeTitles = new();

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
                            SearchTreeEntry entry = new(new(type.Name));
                            entry.level = 1;
                            tree.Add(entry);
                            // nodeTitles.Add(type.Name);
                            // NodeInfoAttribute att = (NodeInfoAttribute)attribute;
                            // var node = Activator.CreateInstance(type);
                        }
                    }
                }
            }
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            return true;
        }
    }
}
