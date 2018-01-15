using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks.Movement;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class PathMove : NavMeshMovement
{
    public SharedGameObjectList waypoints;

    private int waypointIndex;

    public override void OnStart()
    {
        base.OnStart();
        SetDestination(Target());
    }

    public override TaskStatus OnUpdate()
    {
        if(waypoints.Value.Count == 0)
        {
            return TaskStatus.Success;
        }

        if(HasArrived())
        {
            GameObject.Destroy(waypoints.Value[0]);
            waypoints.Value.RemoveAt(0);
            if(waypoints.Value.Count == 0)
            {
                return TaskStatus.Success;
            }

            SetDestination(Target());
        }

        return TaskStatus.Running;
    }

    private Vector3 Target()
    {
        if (waypoints.Value.Count > 0)
            return waypoints.Value[0].transform.position;
        else
            return transform.position;
    }
}
