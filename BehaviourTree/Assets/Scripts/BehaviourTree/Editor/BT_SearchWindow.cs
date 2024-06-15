using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTrees.Editor
{
    public class BT_SearchWindow : ScriptableObject, ISearchWindowProvider
    {
        public BT_GraphView graphView;
        public VisualElement target;

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> tree = new()
            {
                new SearchTreeGroupEntry(new GUIContent("Nodes"))
            };

            List<EntryDataElement> entryDataElements = new();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<Attribute> attributes;
            
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if(type == typeof(Leaf) || type == typeof(Root))
                        continue;

                    attributes = type.GetCustomAttributes().ToList();

                    if(attributes != null && attributes.Count > 0)
                    {
                        var attribute = type.GetCustomAttribute(typeof(NodeInfoAttribute));
                        if(attribute != null)
                        {
                            NodeInfoAttribute nodeInfo = (NodeInfoAttribute)attribute;

                            if(string.IsNullOrEmpty(nodeInfo.Path))
                                continue;
                            
                            var node = Activator.CreateInstance(type);
                            string path = nodeInfo.Path;

                            if(type.IsSubclassOf(typeof(Leaf)))
                            {
                                if(assembly.GetName().Name != nameof(BehaviourTrees))
                                    path += "CustomLeaf/";
                                    
                                path += type.Name;
                                path = path.AddSpaces();
                            }
                            entryDataElements.Add(new(path, node));
                        }
                    }
                }
            }

            entryDataElements.Sort((entryRight, entryLeft) =>
            {
                // Debug.Log(entryLeft.Path + " " + entryRight.Path);
                string[] splitsRight = entryRight.Path.Split('/');
                string[] splitsLeft = entryLeft.Path.Split('/');
                
                if(splitsLeft.Length != splitsRight.Length && splitsLeft.Length == 1)
                    return -1;

                for (int i = 0; i < splitsRight.Length; i++)
                {
                    if(i >= splitsLeft.Length)
                        return 1;

                    int value = splitsRight[i].CompareTo(splitsLeft[i]);
                    if(value != 0)
                        return value;
                }

                return 0;
            });

            List<string> groups = new();
            
            foreach(EntryDataElement dataElement in entryDataElements)
            {
                string[] pathTitles = dataElement.Path.Split('/');
                string groupName = "";

                for (int i = 0; i < pathTitles.Length - 1; i++)
                {
                    groupName += pathTitles[i];
                    if(!groups.Contains(groupName))
                    {
                        tree.Add(new SearchTreeGroupEntry(new(pathTitles[i]), i + 1));
                        groups.Add(groupName);
                    }
                    groupName += "/";
                }

                SearchTreeEntry entry = new SearchTreeEntry(new(pathTitles.Last())) {level = pathTitles.Length, userData = dataElement};
                tree.Add(entry);
            }
            
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var windowMousePosition = graphView.ChangeCoordinatesTo(graphView, context.screenMousePosition - graphView.Window.position.position);
            var graphMousePosition = graphView.contentViewContainer.WorldToLocal(windowMousePosition);

            EntryDataElement dataElement = (EntryDataElement)SearchTreeEntry.userData;
            BehaviourTreeNode node = (BehaviourTreeNode)dataElement.Node;
            node.SetPosition(new(graphMousePosition, new()));
            graphView.Add(node);
            
            return true;
        }
    }

    public struct EntryDataElement
    {
        public string Path {get; private set;}
        public object Node {get; private set;}

        public EntryDataElement(string path, object data)
        {
            Path = path;
            Node = data;
        }
    }
}
