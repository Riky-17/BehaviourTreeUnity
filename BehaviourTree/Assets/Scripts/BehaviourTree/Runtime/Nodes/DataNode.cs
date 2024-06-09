using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees
{
    // [NodeInfo]
    public class DataNode<T> : ParentNode
    {
        readonly Dictionary<string, T> data;
        event Action Callback;

        // public DataNode(string[] keys, T[] values, BehaviourTreeNode child) : base(child)
        // {
        //     data = new();
        //     for (int i = 0; i < keys.Length; i++)
        //         SetData(keys[i], values[i]);
        // }

        // public override NodeStates ChildUpdate() => Children[0].ChildUpdate();

        public bool TryGetData(string key, out T value)
        {
            if (data.ContainsKey(key))
            {
                value = data[key];
                return true;
            }

            value = default;
            return false;
        }

        public bool TryGetData(Action action, string key, out T value)
        {
            if (data.ContainsKey(key))
            {
                Callback += action;
                value = data[key];
                return true;
            }

            value = default;
            return false;
        }

        public void SetData(string key, T value)
        {
            data[key] = value;
            Callback?.Invoke();
        }
        
        public bool SearchKey(string key) => data.ContainsKey(key);
    }
}
