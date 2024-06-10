using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees
{
    [CreateAssetMenu(menuName = "Behavior Tree Graph/New Tree")]
    public class BehaviourTreeGraph : ScriptableObject
    {
        [SerializeReference] Root root;
        public Root Root => root;

        [SerializeReference] List<BehaviourTreeNode> nodes;
        public List<BehaviourTreeNode> Nodes => nodes;

        [SerializeField] List<BehaviourTreeConnection> connections;
        public List<BehaviourTreeConnection> Connections => connections;

        public BehaviourTreeGraph()
        {   
            root = new Root();
            nodes = new();
            connections = new();
        }

        public bool AreNodesConnected(BehaviourTreeNode firstNode, BehaviourTreeNode secondNode)
        {
            List<BehaviourTreeNode> connectedNodes = GetConnectedNodes(firstNode);
            if(connectedNodes.Contains(secondNode))
                return true;
            
            int index = 0;
            List<BehaviourTreeNode> connectedNodesToCurrent;

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

        public List<BehaviourTreeNode> GetNodes(string parentNodeID)
        {
            List<BehaviourTreeNode> nodes = new();

            foreach (BehaviourTreeConnection connection in connections)
            {
                if(connection.parentPort.NodeID == parentNodeID)
                {
                    string childNodeID = connection.childPort.NodeID;
                    nodes.Add(SearchNode(childNodeID));
                }
            }
            return nodes;
        }

        List<BehaviourTreeNode> GetConnectedNodes(BehaviourTreeNode node)
        {
            List<BehaviourTreeNode> connectedNodes = new();

            foreach (BehaviourTreeConnection connection in connections)
            {
                if(connection.parentPort.Node == node)
                    connectedNodes.Add(connection.childPort.Node);
                if(connection.childPort.Node == node)
                    connectedNodes.Add(connection.parentPort.Node);
            }
            return connectedNodes;
        }
    }
}
