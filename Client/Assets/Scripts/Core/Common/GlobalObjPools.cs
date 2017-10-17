using System.Collections;
using System.Collections.Generic;
using Battle;
using Battle.Projectiles;
using UnityEngine;
using Common;

public class GlobalObjPools : Singleton<GlobalObjPools>
{

    private MonoObjPool<ProjectileBase> _LinearProjectilPool;

    public Dictionary<BattleDef.PROJECTILE_TYPE, MonoObjPool<ProjectileBase>> _ProjectilePools;

    public GlobalObjPools()
    {
        GameObject linearProjectile = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Projectiles/JHPProjectileTemplate"));
        ProjectileBase p = linearProjectile.GetComponent<ProjectileBase>();
        _LinearProjectilPool = new MonoObjPool<ProjectileBase>(p, true, 1024);
        _ProjectilePools = new Dictionary<BattleDef.PROJECTILE_TYPE, MonoObjPool<ProjectileBase>>()
        {
            {
                BattleDef.PROJECTILE_TYPE.LINEAR,
                _LinearProjectilPool
            },
            {
                BattleDef.PROJECTILE_TYPE.MISSILE,
                _LinearProjectilPool
            },
            {
                BattleDef.PROJECTILE_TYPE.SECTOR,
                _LinearProjectilPool
            }
        };
    }

    public MonoObjPool<ProjectileBase> GetProjectilePoolByType(BattleDef.PROJECTILE_TYPE type)
    {
        MonoObjPool<ProjectileBase> pool = null;
        _ProjectilePools.TryGetValue(type, out pool);
        return pool;
    }
}
