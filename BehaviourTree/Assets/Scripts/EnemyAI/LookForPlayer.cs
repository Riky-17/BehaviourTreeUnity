using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class LookForPlayer : Leaf
{
    Transform transform;
    Transform target;
    Collider[] colliders;
    float radius = 8;

    public LookForPlayer() {}
    
    // public LookForPlayer(Transform transform) => this.transform = transform;

    public override NodeStates ChildUpdate()
    {
        colliders = Physics.OverlapSphere(transform.position, radius);
        if (colliders.Length > 0)
        {
            foreach (Collider coll in colliders)
            {
                if(coll.gameObject.TryGetComponent(out Player player))
                {
                    target = player.transform;
                    //SetData<Transform>(nameof(target), target);
                    return NodeStates.Success;
                }
            }
        }
        
        target = null;
        //SetData<Transform>(nameof(target), target);
        return NodeStates.Failure;
    }
}
