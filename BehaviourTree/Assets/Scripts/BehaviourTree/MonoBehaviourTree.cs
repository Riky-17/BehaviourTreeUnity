using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class MonoBehaviourTree : MonoBehaviour
    {
        protected Node root = null;

        void Awake() => SetUpTree();

        void Update() => root?.Evaluate();

        public abstract void SetUpTree();
    }
}