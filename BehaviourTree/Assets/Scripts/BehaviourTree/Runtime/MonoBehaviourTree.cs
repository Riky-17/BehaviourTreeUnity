using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class MonoBehaviourTree : MonoBehaviour
    {
        protected BehaviourTreeNode root = null;
        public List<BehaviourTreeNode> Children {get; protected set;} = new();

        void Awake()
        {
            SetUpTree();
            root?.ChildAwake();
        }

        void Start() => root?.ChildStart();
        void OnEnable() => root?.ChildEnable();
        void OnDisable() => root?.ChildDisable();

        void Update() => root?.ChildUpdate();

        public abstract void SetUpTree();
    }
}