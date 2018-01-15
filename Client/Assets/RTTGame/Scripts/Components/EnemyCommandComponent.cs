using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCommandComponent : MonoBehaviour
{
    public List<UnitModeController> patrolEnemys = new List<UnitModeController>();
    public List<GameObject> waypoints = new List<GameObject>();
    //private List<GameObject> enemys = new List<GameObject>();
    //private GameObject waypointParent;
    private void Start()
    {
        //enemys = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        //waypointParent = GameObject.FindGameObjectWithTag("Waypoint");
        StartCoroutine(SwitchPatrol());
    }

    private IEnumerator SwitchPatrol()
    {
        yield return new WaitForFixedUpdate();
        for (int i = 0; i < patrolEnemys.Count; i++)
        {
            UnitModeController unit_enemy = patrolEnemys[i];
            unit_enemy.SwitchMode(UnitMode.PATROL);

            Transform pathParent = waypoints[i].transform;
            List<GameObject> pathPoint = new List<GameObject>();
            foreach(Transform t in pathParent)
            {
                pathPoint.Add(t.gameObject);
            }

            unit_enemy.SetPatrolPath(pathPoint);
        }
    }
}
