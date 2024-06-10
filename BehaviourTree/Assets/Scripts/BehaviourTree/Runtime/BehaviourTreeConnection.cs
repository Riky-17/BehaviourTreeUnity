using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourTrees
{
    [System.Serializable]
    public struct BehaviourTreeConnection
    {
        public BehaviourTreeConnectionPort parentPort;
        public BehaviourTreeConnectionPort childPort;

        public BehaviourTreeConnection(BehaviourTreeConnectionPort parentPort, BehaviourTreeConnectionPort childPort)
        {
            this.parentPort = parentPort;
            this.childPort = childPort;
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
