using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolling : Node
{
    Transform transform;
    List<Vector3> wayPoints;
    float speed = 6;
    int index = 0;

    public EnemyPatrolling(Transform transform, List<Vector3> wayPoints)
    {
        this.transform = transform;
        this.wayPoints = wayPoints;
    }

    public override NodeStates Evaluate()
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
