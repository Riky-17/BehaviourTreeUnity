using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees
{
    public class BehaviourTree : MonoBehaviour
    {
        [SerializeField] BehaviourTreeGraph graph;

        BehaviourTreeNode root = null;

        void Awake()
        {
            SetUpTree();
            root?.ChildAwake();
        }

        void Start() => root?.ChildStart();
        void OnEnable() => root?.ChildEnable();
        void OnDisable() => root?.ChildDisable();

        void Update() => root?.ChildUpdate();

        void SetUpTree()
        {
            root = graph.Nodes[0];
            PopulateTree(root);
        }

        void PopulateTree(BehaviourTreeNode node)
        {
            List<string> childrenID = node.PopulateChildren(graph);
            
            if (childrenID == null || childrenID.Count == 0)
                return;

            foreach (string nextNodeID in childrenID)
            {
                BehaviourTreeNode nextNode = graph.SearchNode(nextNodeID);
                PopulateTree(nextNode);
            }
        }


    }
}