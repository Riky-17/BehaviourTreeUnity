using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
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
        public string nodeID; 
        public int portIndex; 

        public BehaviourTreeConnectionPort(string nodeID, int portIndex)
        {
            this.nodeID = nodeID;
            this.portIndex = portIndex;
        }
    }
}
