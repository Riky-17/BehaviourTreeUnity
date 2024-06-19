using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees
{
    public class DebugNode : Leaf
    {
        [Flags]
        enum Methods 
        {
            Awake = 1,
            OnEnable = 2,
            Start = 4,
            Update = 8,
            OnDisable = 16
        }

        [ShowField, SerializeField] string text;
        [ShowField, SerializeField] List<string> texts;
        [ShowField, SerializeField] Methods methods;
        [ShowField, SerializeField] NodeStates state;

        public override void ChildAwake()
        {
            if((methods & Methods.Awake) != 0)
                Debug.Log("Awake: " + text);
        }

        public override void ChildEnable()
        {
            if((methods & Methods.OnEnable) != 0)
                Debug.Log("On Enable: " + text);
        }

        public override void ChildStart()
        {
            if((methods & Methods.Start) != 0)
                Debug.Log("Start: " + text);
        }

        public override NodeStates ChildUpdate()
        {
            if((methods & Methods.Update) != 0)
                Debug.Log("Update: " + text);
            
            return state;
        }

        public override void ChildDisable()
        {
            if((methods & Methods.OnDisable) != 0)
                Debug.Log("On Disable: " + text);
        }
    }
}
