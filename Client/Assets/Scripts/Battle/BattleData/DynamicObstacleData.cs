using System.Collections;
using System.Collections.Generic;
using Battle;
using Battle.Projectiles;
using UnityEngine;
using Common;

public class DynamicObstacleData : MonoBase
{
    public virtual void OnProjectileCollide(ProjectileBase projectile, Vector3 hitPoint,
            PROJECTILE_HITTYPE hitType, CommEnum.PROJECTILE_TYPE pType)
    {
        
    }
}
