using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class ChasePlayer : Leaf
{
    Transform transform;
    Transform target;
    float minDist = 1f;
    float speed;

    public ChasePlayer(Transform transform) => this.transform = transform;

    public override void ChildStart() => speed = GetData<float>(nameof(speed), OnSpeedChange);

    public override NodeStates ChildUpdate()
    {
        target = GetData<Transform>(nameof(target));
        Vector3 posToTarget = target.position - transform.position;

        if(posToTarget.magnitude <= minDist)
            return NodeStates.Success;

        Vector3 dir = posToTarget.normalized;
        transform.position += speed * Time.deltaTime * dir;

        return NodeStates.Running;
    }

    void OnSpeedChange() => speed = GetData<float>(nameof(speed));
}
