using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Solider : LivingEntity
{
    private NavMeshAgent pathfinder;
    protected override void OnAwake()
    {
        base.OnAwake();
        pathfinder = GetComponent<NavMeshAgent>();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    public void SetDestination(Vector3 pos)
    {
        pathfinder.SetDestination(pos);
    }

    float refreshRate = 0.25f;
    Vector3 targetPosition;
    Vector3 dirToTarget;
    WaitForSeconds updateWait;
    IEnumerator UpdatePath()
    {
        updateWait = new WaitForSeconds(refreshRate);
        while(true)
        {
            yield return updateWait;
        }
    }
}
