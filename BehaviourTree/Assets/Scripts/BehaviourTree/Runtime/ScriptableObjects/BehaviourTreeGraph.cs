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

        public bool AreNodesConnected(BehaviourTreeNode firstNode, BehaviourTreeNode secondNode)
        {
            List<BehaviourTreeNode> connectedNodes = GetConnectedNodes(firstNode);
            if(connectedNodes.Contains(secondNode))
                return true;
            
            int index = 0;
            List<BehaviourTreeNode> connectedNodesToCurrent = new();

            while(true)
            {
                int listLength = connectedNodes.Count;
                if(index == listLength)
                    return false;

                BehaviourTreeNode currentNode = connectedNodes[index];
                connectedNodesToCurrent = GetConnectedNodes(currentNode);
                
                foreach (BehaviourTreeNode node in connectedNodesToCurrent)
                {
                    if(connectedNodes.Contains(node))
                        continue;
                    
                    if(node == secondNode)
                        return true;
                    
                    connectedNodes.Add(node);
                }
                index++;
            }
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
                if(connection.outputPort.NodeID == outputNodeID)
                {
                    string inputNodeID = connection.inputPort.NodeID;
                    nodes.Add(SearchNode(inputNodeID));
                }
            }
            return nodes;
        }

        List<BehaviourTreeNode> GetConnectedNodes(BehaviourTreeNode node)
        {
            List<BehaviourTreeNode> connectedNodes = new();

            foreach (BehaviourTreeConnection connection in connections)
            {
                if(connection.inputPort.Node == node)
                    connectedNodes.Add(connection.outputPort.Node);
                if(connection.outputPort.Node == node)
                    connectedNodes.Add(connection.inputPort.Node);
            }
            return connectedNodes;
        }
    }
}
