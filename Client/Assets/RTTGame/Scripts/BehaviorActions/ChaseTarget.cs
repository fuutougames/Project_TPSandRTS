using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class ChaseTarget : Action
{
    public SharedGameObject chasetarget;

    public override void OnStart()
    {
        base.OnStart();

        UnitModeController unitMode = this.gameObject.GetComponent<UnitModeController>();
        unitMode.SwitchMode(UnitMode.CHASE);
        if(chasetarget.Value != null)
        {
            unitMode.SetChaseTarget(chasetarget.Value);
        }       
    }
}
