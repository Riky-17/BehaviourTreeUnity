using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees
{
    [CreateAssetMenu(menuName = "Behavior Tree Graph/New Tree")]
    public class BehaviourTreeGraph : ScriptableObject
    {
        [SerializeReference] List<BehaviourTreeNode> nodes;
        public List<BehaviourTreeNode> Nodes => nodes;
        [SerializeField] List<BehaviourTreeConnection> connections;
        public List<BehaviourTreeConnection> Connections => connections;

        public BehaviourTreeGraph()
        {
            nodes = new();
            connections = new();
        }

        public BehaviourTreeNode SearchNode(string nodeID)
        {
            foreach (BehaviourTreeNode node in nodes)
            {
                if (node.ID == nodeID)
                    return node;
            }
            return null;
        }

        public List<BehaviourTreeNode> GetNodes(string outputNodeID)
        {
            List<BehaviourTreeNode> nodes = new();

            foreach (BehaviourTreeConnection connection in connections)
            {
                if(connection.outputPort.nodeID == outputNodeID)
                {
                    string inputNodeID = connection.inputPort.nodeID;
                    nodes.Add(SearchNode(inputNodeID));
                }
            }
            return nodes;
        }
    }
}
