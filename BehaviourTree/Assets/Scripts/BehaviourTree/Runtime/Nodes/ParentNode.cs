using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees
{
    public abstract class ParentNode : BehaviourTreeNode
    {
        protected List<BehaviourTreeNode> Children {get; private set;}

        public override void ChildStart()
        {
            foreach (BehaviourTreeNode child in Children)
                child.ChildStart();
        }

        public override void ChildEnable()
        {
            foreach (BehaviourTreeNode child in Children)
                child.ChildEnable();
        }

        public override void ChildDisable()
        {
            foreach (BehaviourTreeNode child in Children)
                child.ChildDisable();
        }

        public override List<string> PopulateChildren(BehaviourTreeGraph graph)
        {
            Children = graph.GetNodes(ID);

            if (Children.Count == 0)
                return null;

            if (Children.Count > 1)
                {
                    Children.Sort((nodeRight, nodeLeft) =>
                    {
                        float nodeRightPosX = nodeRight.Position.x;
                        float nodeLeftPosX = nodeLeft.Position.x;

                        int value = nodeRightPosX.CompareTo(nodeLeftPosX);
                        return value;
                    });
                }

            List<string> childrenID = new();
            
            foreach (BehaviourTreeNode node in Children)
                childrenID.Add(node.ID);

            return childrenID;
        }
    }
}