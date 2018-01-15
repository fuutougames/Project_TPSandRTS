using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class SetUnitTarget : Action
{
    public SharedGameObject target;
    private GameObject go_target;
    private Plane groundPlane;

    public override void OnAwake()
    {
        base.OnAwake();
        groundPlane = new Plane(Vector3.up, Vector3.up * 0.58f);
        go_target = new GameObject();
    }

    private Ray ray;
    private float rayDistance;
    private Vector3 hitPoint;
    public override TaskStatus OnUpdate()
    {
        //Debug.Log("SetTargetPos OnUpdate");
        if(Input.GetMouseButtonUp(1))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (groundPlane.Raycast(ray, out rayDistance))
            {
                hitPoint = ray.GetPoint(rayDistance);
                go_target.transform.position = hitPoint;
                target.Value = go_target;
                return TaskStatus.Failure;
            }
        }
        return TaskStatus.Running;
    }
}
