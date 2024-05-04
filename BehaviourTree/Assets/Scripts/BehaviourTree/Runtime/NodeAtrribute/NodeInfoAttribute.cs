using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class NodeInfoAttribute : Attribute
    {
        public string Path {get; private set;}

        public NodeInfoAttribute(string path) => Path = path;
    }
}
