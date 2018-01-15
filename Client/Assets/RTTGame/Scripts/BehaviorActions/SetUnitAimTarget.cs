using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class SetUnitAimTarget : Action
{
    public SharedGameObject aimtarget;
    private Ray ray;
    private RaycastHit hitInfo;
    public override TaskStatus OnUpdate()
    {
        if(Input.GetMouseButtonDown(1))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hitInfo))
            {
                if(hitInfo.transform.tag == "Enemy")
                {
                    aimtarget.Value = hitInfo.transform.gameObject;
                    return TaskStatus.Failure;
                }
                else
                {
                    return TaskStatus.Running;
                }
            }
        }
        return TaskStatus.Running;
    }
}
