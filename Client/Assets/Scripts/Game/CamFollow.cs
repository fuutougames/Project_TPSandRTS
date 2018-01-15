using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBase
{
    public bool following = false;
    public Transform target;

    protected override void OnLateUpdate()
    {
        base.OnLateUpdate();
        if (!following) return;
        if (target == null) return;
        Vector3 targetPos = target.position;
        targetPos.y = 0;
        this.transform.position = targetPos;
    }
}
