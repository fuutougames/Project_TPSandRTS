using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battle;
using Battle.Data;
using Battle.Guns;
using UnityEngine.UI;

public class TestRoutine : MonoBehaviour
{
    public GunBase Gun;

	// Use this for initialization
	void Start () {
		GameObject obstacleRoot = GameObject.Find("ObstacleRoot");
        StaticObstacleData[] obstacles = obstacleRoot.transform.GetComponentsInChildren<StaticObstacleData>(true);
        for (int i = 0; i < obstacles.Length; ++i)
        {
            obstacles[i].Initialize();
            BattleMgr.Instance.BData.SceneData.RegisterObstacle(obstacles[i]);
        }

        GameObject characterRoot = GameObject.Find("PlayerRoot");
        Pawn[] characters = characterRoot.transform.GetComponentsInChildren<Pawn>(true);
        for (int i = 0; i < characters.Length; ++i)
        {
            BattleMgr.Instance.BData.RegisterPawn(characters[i]);
        }

	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Gun.StartAttack();
        }
	    if (Input.GetKeyUp(KeyCode.Mouse0))
	    {
	        Gun.CancelAttack();
	    }
	}
}
