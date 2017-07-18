using System.Collections;
using System.Collections.Generic;
using Battle;
using Battle.Projectiles;
using UnityEngine;

public class DynamicObstacleData : MonoBase
{
    public virtual void OnProjectileCollide(ProjectileBase projectile, Vector3 hitPoint,
            BattleDef.PROJECTILE_HITTYPE hitType, BattleDef.PROJECTILE_TYPE pType)
    {
        
    }
}
