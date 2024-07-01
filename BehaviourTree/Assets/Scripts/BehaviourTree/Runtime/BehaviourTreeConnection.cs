using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourTrees
{
    [System.Serializable]
    public class BehaviourTreeConnection
    {
        [SerializeReference] public BehaviourTreeNode parentNode;
        [SerializeReference] public List<BehaviourTreeNode> childrenNodes;
        public int ChildrenCount => childrenNodes.Count;

        public BehaviourTreeConnection(BehaviourTreeNode parentNode, List<BehaviourTreeNode> childrenNodes)
        {
            this.parentNode = parentNode;
            this.childrenNodes = childrenNodes;
        }

        public BehaviourTreeConnection(BehaviourTreeNode parentNode, BehaviourTreeNode child)
        {
            this.parentNode = parentNode;
            childrenNodes = new() { child };
        }

        public bool ContainsChild(BehaviourTreeNode child) => childrenNodes.Contains(child);
        public void AddChild(BehaviourTreeNode child) => childrenNodes.Add(child);
        public void RemoveChild(BehaviourTreeNode child) => childrenNodes.Remove(child);
    }
}
