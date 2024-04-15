using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BehaviourTree
{
    [SerializeField] List<Vector3> wayPoints;

    public override void SetUpTree()
    {
        root = new Selector
        (
            new EnemyPatrolling(transform, wayPoints)
        );
    }

}
