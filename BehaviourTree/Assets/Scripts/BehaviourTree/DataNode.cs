using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class DataNode<T> : ParentNode
    {
        Dictionary<string, T> data;

        public DataNode(string[] keys, T[] values, Node child) : base(child)
        {
            data = new();
            for (int i = 0; i < keys.Length; i++)
                SetData(keys[i], values[i]);
        }

        public override NodeStates Evaluate() => Children[0].Evaluate();

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

        public bool SearchKey(string key) => data.ContainsKey(key);
        public void SetData(string key, T value) => data[key] = value;
    }
}
