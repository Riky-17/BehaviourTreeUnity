using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class Enemy : MonoBehaviourTree
{
    [SerializeField] List<Vector3> wayPoints;

    public override void SetUpTree()
    {
        root = new DataNode<float>
        (
            new string[] {"speed"},
            new float[] {6},
            new Selector
            (
                new DataNode<Transform>
                (
                    new string[] {"target"},
                    new Transform[] {null},
                    new Sequence
                    (
                        new LookForPlayer(transform),
                        new ChasePlayer(transform)
                    )
                ),
                new EnemyPatrolling(transform, wayPoints)
            )
        );
    }
}
