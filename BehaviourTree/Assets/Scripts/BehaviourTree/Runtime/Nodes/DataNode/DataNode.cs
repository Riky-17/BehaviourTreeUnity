using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees
{
    [NodeInfo("Parent Node/Data Node")]
    public class DataNode : ParentNode
    {
        public List<DataNodeEntry> entries  = new();

        private readonly Dictionary<string, object> data = new(); 

        public bool TryGetValue<T>(string key, out T value)
        {
            if(data.TryGetValue(key, out object dataValue) && dataValue is DataNodeValue<T> castedValue)
            {
                value = castedValue.Value;
                return true;
            }

            value = default;
            return false;
        }

        public void SetValuesOnDataNode()
        {
            foreach (DataNodeEntry entry in entries)
            {
                entry.SetValueOnDataNode(this);
            }
        }

        public void SetValue<T>(string key, T value) => data[key] = new DataNodeValue<T>(value);

        public bool HasKey(string key) => data.ContainsKey(key);
    }

    public struct DataNodeValue<T>
    {
        public T Value;
        public Type ValueType;

        public DataNodeValue(T value)
        {
            Value = value;
            ValueType = typeof(T);
        }
    }
}
