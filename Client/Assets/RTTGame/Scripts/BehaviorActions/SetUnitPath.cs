using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class SetUnitPath : Action
{
    public SharedGameObjectList waypoints;
    private Plane groundPlane;

    public override void OnAwake()
    {
        base.OnAwake();
        groundPlane = new Plane(Vector3.up, Vector3.up * 0.58f);
    }

    private Ray ray;
    private float rayDistance;
    private Vector3 hitPoint;
    public override TaskStatus OnUpdate()
    {
        if (Input.GetMouseButtonUp(1))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (groundPlane.Raycast(ray, out rayDistance))
            {
                hitPoint = ray.GetPoint(rayDistance);
                GameObject go_wayPoint = new GameObject();
                go_wayPoint.transform.position = hitPoint;
                waypoints.Value.Add(go_wayPoint);
                return TaskStatus.Failure;
            }
        }
        return TaskStatus.Running;
    }
}
