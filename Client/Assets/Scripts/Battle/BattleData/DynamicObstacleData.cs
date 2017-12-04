using System.Collections;
using System.Collections.Generic;
using Battle;
using Battle.Projectiles;
using UnityEngine;

public class DynamicObstacleData : MonoBase
{
    public virtual void OnProjectileCollide(ProjectileBase projectile, Vector3 hitPoint,
            PROJECTILE_HITTYPE hitType, PROJECTILE_TYPE pType)
    {
        
    }
}
