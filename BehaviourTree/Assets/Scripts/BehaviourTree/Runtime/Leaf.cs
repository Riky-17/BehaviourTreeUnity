using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class Leaf : Node 
    {
        public Leaf() : base() {}

        protected T GetData<T>(string key)
        {
            Node node = parent;

            while (true)
            {
                if (node == null)
                    return default;

                if (node is DataNode<T>)
                {
                    DataNode<T> dataNode = node as DataNode<T>;

                    if (dataNode.TryGetData(key, out T value))
                        return value;
                    
                    node = node.parent;
                    continue;

                }

                node = node.parent;
                continue;
            }
        }

        protected T GetData<T>(string key, Action action)
        {
            Node node = parent;

            while (true)
            {
                if (node == null)
                    return default;

                if (node is DataNode<T>)
                {
                    DataNode<T> dataNode = node as DataNode<T>;

                    if (dataNode.TryGetData(action, key, out T value))
                        return value;
                    
                    node = node.parent;
                    continue;

                }

                node = node.parent;
                continue;
            }
        }

        protected void SetData<T>(string key, T value)
        {
            Node node = parent;

            while (true)
            {
                if (node == null)
                    return;

                if (node is DataNode<T>)
                {
                    DataNode<T> dataNode = node as DataNode<T>;

                    if (dataNode.SearchKey(key))
                        dataNode.SetData(key, value);
                    
                    node = node.parent;
                    continue;
                }

                node = node.parent;
                continue;
            }
        }
    }
}