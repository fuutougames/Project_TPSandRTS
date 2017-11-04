using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Projectiles
{
    public class JHP_Projectile : LinearProjectile
    {

        /// <summary>
        /// JHP bullet should lost all damage when hit an obstacle
        /// </summary>
        /// <param name="obstacle"></param>
        /// <param name="penLen"></param>
        /// <returns></returns>
        protected override float CalcDmgLost(StaticObstacleData obstacle, float penLen)
        {
            return _PBData.BaseDamage;
        }


        public float CalculateDamage(PawnHitData hitData, float remainDmg)
        {
            // fire from friendly unit
            if (hitData.APawn.Data.Side == SyncSide && SyncSide != 0 && !BattleMgr.Instance.BData.FriendlyFire)
            {
                return 0;
            }

            float diff = _PBData.Penetration - hitData.APawn.Data.Armor;
            if (diff > 0)
            {
                // armor penetrated
                // make JHP penetration damage
                return remainDmg * (1.0f + diff / _PBData.Penetration);
            }
            else
            {
                // not penetrate
                // make impact damage only
                return remainDmg * BattleConst.JHP_NOT_PENETRATE_DMG_WEIGHT;
            }
        }

        public override bool ProcessHitData(List<PawnHitData> hitData, int hitDataLen, out int hitCnt)
        {
            hitCnt = 0;
            if (hitDataLen > 0)
            {
                hitCnt = 1;
                float distance = hitData[0].HitDistance;
                float remainDmg = _DmgLine.GetRemainDmgByCurMagnitude(distance);
                float damageMake = CalculateDamage(hitData[0], remainDmg);
                if (hitData[0].APawn.Data.Armor < _PBData.Penetration)
                    hitData[0].APawn.TakeDamage(damageMake, BattleDef.DAMAGE_TYPE.BULLET_PENETRATE, hitData[0].HitPoints);
                else
                    hitData[0].APawn.TakeDamage(damageMake, BattleDef.DAMAGE_TYPE.BULLET_IMPACT, hitData[0].HitPoints);
                RealRange = Vector3.Distance(_SyncStartPos, hitData[0].HitPoints[0]);
            }
            return true;
        }
    }
}
