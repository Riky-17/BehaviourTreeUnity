using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class MonoBehaviourTree : MonoBehaviour
    {
        protected Node root = null;

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