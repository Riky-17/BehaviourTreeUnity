using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees
{
    [Serializable]
    public class DataNodeEntry : ISerializationCallbackReceiver
    {
        public string key;
        public AnyValue.ValueTypes valueType;
        public AnyValue value;

        static Dictionary<AnyValue.ValueTypes, Action<DataNode, string, AnyValue>> dispatchTable = new()
        {
            { AnyValue.ValueTypes.Int, (dataNode, keyName, anyValue) => dataNode.SetValue<int>(keyName, anyValue)},
            { AnyValue.ValueTypes.Float, (dataNode, keyName, anyValue) => dataNode.SetValue<float>(keyName, anyValue)},
            { AnyValue.ValueTypes.Bool, (dataNode, keyName, anyValue) => dataNode.SetValue<bool>(keyName, anyValue)},
            { AnyValue.ValueTypes.String, (dataNode, keyName, anyValue) => dataNode.SetValue<string>(keyName, anyValue)},
            { AnyValue.ValueTypes.Vector3, (dataNode, keyName, anyValue) => dataNode.SetValue<Vector3>(keyName, anyValue)},
            { AnyValue.ValueTypes.GameObject, (dataNode, keyName, anyValue) => dataNode.SetValue<GameObject>(keyName, anyValue)},
            { AnyValue.ValueTypes.Transform, (dataNode, keyName, anyValue) => dataNode.SetValue<Transform>(keyName, anyValue)}
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
            Vector3,
            GameObject,
            Transform
        }

        public ValueTypes type;

        public int intValue;
        public float floatValue;
        public bool boolValue;
        public string stringValue;
        public Vector3 vector3Value;
        public GameObject gameObjectValue;
        public Transform transformValue;

        public static implicit operator int(AnyValue value) => value.ConvertType<int>();
        public static implicit operator float(AnyValue value) => value.ConvertType<float>();
        public static implicit operator bool(AnyValue value) => value.ConvertType<bool>();
        public static implicit operator string(AnyValue value) => value.ConvertType<string>();
        public static implicit operator Vector3(AnyValue value) => value.ConvertType<Vector3>();
        public static implicit operator GameObject(AnyValue value) => value.ConvertType<GameObject>();
        public static implicit operator Transform(AnyValue value) => value.ConvertType<Transform>();

        T ConvertType<T>()
        {
            return type switch
            {
                ValueTypes.Int => AsInt<T>(intValue),
                ValueTypes.Float => AsFloat<T>(floatValue),
                ValueTypes.Bool => AsBool<T>(boolValue),
                ValueTypes.String => (T)(object)stringValue,
                ValueTypes.Vector3 => (T)(object)vector3Value,
                ValueTypes.GameObject => (T)(object)gameObjectValue,
                ValueTypes.Transform => (T)(object)transformValue,
                _ => throw new NotSupportedException($"Not supported value type {typeof(T)}")
            };
        }

        T AsInt<T>(int value) => typeof(T) == typeof(int) && value is T correctType ? correctType : default;
        T AsFloat<T>(float value) => typeof(T) == typeof(float) && value is T correctType ? correctType : default;
        T AsBool<T>(bool value) => typeof(T) == typeof(bool) && value is T correctType ? correctType : default;
    }
}
