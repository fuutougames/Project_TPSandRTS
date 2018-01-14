using System.Collections;
using System.Collections.Generic;
using Battle;
using Battle.Projectiles;
using UnityEngine;
using Common;

public class GlobalObjPools : Singleton<GlobalObjPools>
{

    private MonoObjPool<ProjectileBase> _LinearProjectilPool;

    public Dictionary<CommEnum.PROJECTILE_TYPE, MonoObjPool<ProjectileBase>> _ProjectilePools;

    public GlobalObjPools()
    {
        GameObject linearProjectile = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Projectiles/JHPProjectileTemplate"));
        ProjectileBase p = linearProjectile.GetComponent<ProjectileBase>();
        _LinearProjectilPool = new MonoObjPool<ProjectileBase>(p, true, 1024);
        _ProjectilePools = new Dictionary<CommEnum.PROJECTILE_TYPE, MonoObjPool<ProjectileBase>>()
        {
            {
                //BattleDef.PROJECTILE_TYPE.LINEAR,
                CommEnum.PROJECTILE_TYPE.JHP_PROJECTILE,
                _LinearProjectilPool
            },
            {
                CommEnum.PROJECTILE_TYPE.AP_PROJECTILE,
                _LinearProjectilPool
            },
            {
                CommEnum.PROJECTILE_TYPE.MISSILE,
                _LinearProjectilPool
            },
            {
                CommEnum.PROJECTILE_TYPE.SECTOR,
                _LinearProjectilPool
            }
        };
    }

    public MonoObjPool<ProjectileBase> GetProjectilePoolByType(CommEnum.PROJECTILE_TYPE type)
    {
        MonoObjPool<ProjectileBase> pool = null;
        _ProjectilePools.TryGetValue(type, out pool);
        return pool;
    }
}
