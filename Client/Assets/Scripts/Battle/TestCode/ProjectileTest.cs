using System.Collections;
using System.Collections.Generic;
using Battle;
using Battle.Data;
using Battle.Projectiles;
using UnityEngine;

public class ProjectileTest : MonoBehaviour {
    public Transform CachedTransform { get; private set; }
    public float ShootInterval = 1.0f;
    private float _AccumTime = .0f;
    public float Velocity = 400.0f;
    public float MaxRange = 1000;
    public float Penetration = 5;
    public LinearProjectile projectile;
    private ProjectileBattleData data;
    public bool Triggered = false;

    void Awake()
    {
        CachedTransform = transform;
        data = new ProjectileBattleData();
        data.PType = BattleDef.PROJECTILE_TYPE.LINEAR;
        data.BaseDamage = 100.0f;
        data.Velocity = Velocity;
        data.MaxRange = MaxRange;
        data.Penetration = Penetration;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    if (!Triggered)
	        return;
	    if (_AccumTime < ShootInterval)
	    {
            // do nothing
	        _AccumTime += Time.deltaTime;
	        return;
	    }

        // data setup 
        data.Velocity = Velocity;
        data.MaxRange = MaxRange;
        data.Penetration = Penetration;

        _AccumTime = .0f;
	    // do shoot;
	    ProjectileBase ins = GameObject.Instantiate(projectile.gameObject).GetComponent<ProjectileBase>();
        ins.Init(data);
        ins.TriggerProjectile(CachedTransform.position, CachedTransform.forward, 0);
    }

    public void TriggerProjectiles()
    {
        Triggered = !Triggered;
        _AccumTime = 0;
    }

    public void RegisterObjects()
    {
        GameObject obstacleRoot = GameObject.Find("ObstacleRoot");
        StaticObstacleData[] obstacles = obstacleRoot.transform.GetComponentsInChildren<StaticObstacleData>(true);
        for (int i = 0; i < obstacles.Length; ++i)
        {
            obstacles[i].Initialize();
            BattleMgr.Instance.SceneData.RegisterObstacle(obstacles[i]);
        }
    }

    public void RegisterCharacters()
    {
        GameObject characterRoot = GameObject.Find("PlayerRoot");
        Pawn[] characters = characterRoot.transform.GetComponentsInChildren<Pawn>(true);
        for (int i = 0; i < characters.Length; ++i)
        {
            BattleMgr.Instance.BData.RegisterPawn(characters[i]);
        }
    }
}
