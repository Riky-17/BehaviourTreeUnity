using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees
{
    public class NodeInfoAttribute : Attribute
    {
        public string Path {get; private set;}
        public bool HasParent {get; private set;}
        public bool HasChild {get; private set;}
        public bool HasMultipleChildren = false;

        public NodeInfoAttribute(string path, bool hasParent = true, bool hasChild = true)
        {
            Path = path;
            HasParent = hasParent;
            HasChild = hasChild;
        }
    }
}
