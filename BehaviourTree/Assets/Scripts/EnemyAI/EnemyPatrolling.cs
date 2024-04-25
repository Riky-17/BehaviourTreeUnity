using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class EnemyPatrolling : Leaf
{
    
    Transform transform;
    List<Vector3> wayPoints;
    int speed;
    int index = 0;

    public EnemyPatrolling(Transform transform, List<Vector3> wayPoints)
    {
        this.transform = transform;
        this.wayPoints = wayPoints;
    }

    public override void ChildStart() => speed = GetData<int>(nameof(speed));

    public override NodeStates ChildUpdate()
    {
        Vector3 tfToPoint = wayPoints[index] - transform.position;
        if(tfToPoint.magnitude <= .5f)
        {
            index = index == wayPoints.Count - 1 ? 0 : index + 1;
            tfToPoint = wayPoints[index] - transform.position;
        }
        transform.position += speed * Time.deltaTime * tfToPoint.normalized;

        return NodeStates.Running;
    }
}
