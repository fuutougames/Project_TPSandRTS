using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class ReturnPatrol : Action
{
    public override void OnStart()
    {
        base.OnStart();
        UnitModeController unitMode = this.gameObject.GetComponent<UnitModeController>();
        unitMode.SwitchMode(UnitMode.PATROL);
    }
}
