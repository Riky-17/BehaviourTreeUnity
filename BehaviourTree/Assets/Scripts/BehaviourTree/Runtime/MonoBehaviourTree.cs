using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class MonoBehaviourTree : MonoBehaviour
    {
        protected BehaviourTreeNode root = null;
        [HideInInspector] [SerializeReference] List<BehaviourTreeNode> children = new();
        public List<BehaviourTreeNode> Children => children;
        [HideInInspector] [SerializeField] List<BehaviourTreeConnection> connections = new();
        public List<BehaviourTreeConnection> Connections => connections;

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