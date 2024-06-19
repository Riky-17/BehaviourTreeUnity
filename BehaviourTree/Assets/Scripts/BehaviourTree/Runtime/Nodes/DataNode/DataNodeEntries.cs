using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees
{
    public class DataNodeEntries
    {
        public string test;
        readonly DataNode dataNode;
        public List<DataNodeEntryData> entries;

        public DataNodeEntries(DataNode dataNode)
        {
            this.dataNode = dataNode;
            entries = new();
        }

        public void SetValuesOnDataNode()
        {
            foreach (DataNodeEntryData entry in entries)
            {
                entry.SetValueOnDataNode(dataNode);
            }
        }
    }

    [Serializable]
    public class DataNodeEntryData : ISerializationCallbackReceiver
    {
        public string key;
        public AnyValue.ValueTypes valueType;
        public AnyValue value;

        static Dictionary<AnyValue.ValueTypes, Action<DataNode, string, AnyValue>> dispatchTable = new()
        {
            { AnyValue.ValueTypes.Bool, (dataNode, keyName, anyValue) => dataNode.SetValue<bool>(keyName, anyValue)}
        };

        public void SetValueOnDataNode(DataNode dataNode) => dispatchTable[value.type](dataNode, key, value);


        public void OnAfterDeserialize()
        {
            value.type = valueType;
        }

        public void OnBeforeSerialize()
        {
            
        }
    }

    [Serializable]
    public struct AnyValue
    {
        public enum ValueTypes
        {
            Int,
            Float,
            Bool,
            String,
            Vector3
        }

        public ValueTypes type;

        public int intValue;
        public float floatValue;
        public bool boolValue;
        public string stringValue;
        public Vector3 vector3Value;

        public static implicit operator bool(AnyValue value) => value.ConvertType<bool>();

        T ConvertType<T>()
        {
            return type switch
            {
                ValueTypes.Bool => AsBool<T>(boolValue),
                _ => throw new NotSupportedException($"Not supported value type {typeof(T)}")
            };
        }

        T AsBool<T>(bool value) => typeof(T) == typeof(bool) && value is T correctType ? correctType : default;
    }
}
