using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees
{
    public class NodeInfoAttribute : Attribute
    {
        public string Path {get; private set;}
        public bool HasInput {get; private set;}
        public bool HasOutput {get; private set;}
        public bool HasMultipleOutput;

        public NodeInfoAttribute(string path, bool hasInput = true, bool hasOutput = true)
        {
            Path = path;
            HasInput = hasInput;
            HasOutput = hasOutput;
        }
    }
}
