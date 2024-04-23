using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class ChasePlayer : Leaf
{
    Transform transform;
    Transform target;
    float minDist = 1f;
    int speed;

    public ChasePlayer(Transform transform) => this.transform = transform;

    public override NodeStates Evaluate()
    {
        target = GetData<Transform>(nameof(target));
        Vector3 posToTarget = target.position - transform.position;

        if(posToTarget.magnitude <= minDist)
            return NodeStates.Success;

        speed = GetData<int>(nameof(speed));
        Vector3 dir = posToTarget.normalized;
        transform.position += speed * Time.deltaTime * dir;

        return NodeStates.Running;
    }
}
