using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourTrees
{
    [System.Serializable]
    public struct BehaviourTreeConnection
    {
        public BehaviourTreeConnectionPort inputPort;
        public BehaviourTreeConnectionPort outputPort;

        public BehaviourTreeConnection(BehaviourTreeConnectionPort inputPort, BehaviourTreeConnectionPort outputPort)
        {
            this.inputPort = inputPort;
            this.outputPort = outputPort;
        }
    }

    [System.Serializable]
    public struct BehaviourTreeConnectionPort
    {
        [SerializeReference] public BehaviourTreeNode Node;
        public string NodeID;

        public BehaviourTreeConnectionPort(BehaviourTreeNode node)
        {
            Node = node;
            NodeID = node.ID;
        }
    }
}
