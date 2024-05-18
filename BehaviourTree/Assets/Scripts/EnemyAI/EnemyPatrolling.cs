using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class EnemyPatrolling : Leaf
{
    
    Transform transform;
    List<Vector3> wayPoints;
    float speed;
    int index = 0;

    public EnemyPatrolling(Transform transform, List<Vector3> wayPoints)
    {
        this.transform = transform;
        this.wayPoints = wayPoints;
    }

    public override void ChildStart() => speed = GetData<float>(nameof(speed), OnSpeedChange);

    public override NodeStates ChildUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            SetData<float>(nameof(speed), 20);

        Vector3 tfToPoint = wayPoints[index] - transform.position;
        if(tfToPoint.magnitude <= .5f)
        {
            index = index == wayPoints.Count - 1 ? 0 : index + 1;
            tfToPoint = wayPoints[index] - transform.position;
        }
        transform.position += speed * Time.deltaTime * tfToPoint.normalized;

        return NodeStates.Running;
    }

    void OnSpeedChange() => speed = GetData<float>(nameof(speed));
}
