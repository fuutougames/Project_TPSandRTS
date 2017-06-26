using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;

public class ProjectileTest : MonoBehaviour {
    public Transform CachedTransform { get; private set; }
    public float ShootInterval = 1.0f;
    private float _AccumTime = .0f;
    public float Velocity = 400.0f;
    public float MaxRange = 1000;
    public float Penetration = 5;
    public LineProjectile projectile;
    private ProjectileBattleData data;

    void Awake()
    {
        CachedTransform = transform;
        data = new ProjectileBattleData();
        data.PType = BattleDef.PROJECTIL_TYPE.LINEAR;
        data.BaseDamage = 100.0f;
        data.Velocity = Velocity;
        data.MaxRange = MaxRange;
        data.Penetration = Penetration;
    }
	
	// Update is called once per frame
	void Update ()
	{
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
        projectile.Init(data);
        projectile.TriggerProjectile();
    }
}
