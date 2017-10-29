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


        public override float CalculateDamage(PawnHitData hitData, float remainDmg)
        {
            // fire from friendly unit
            if (hitData.APawn.Side == SyncSide && SyncSide != 0 && !BattleMgr.Instance.BData.FriendlyFire)
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
                return remainDmg * .375f;
            }
        }

        public override bool ProcessHitData(List<PawnHitData> hitData, int hitDataLen, out int hitCnt)
        {
            hitCnt = 0;
            if (hitDataLen > 0)
            {
                hitCnt = 1;
                float distance = Vector3.Distance(hitData[0].HitPoints[0], _SyncStartPos);
                float remainDmg = _DmgLine.GetRemainDmgByCurMagnitude(distance);
                float damageMake = CalculateDamage(hitData[0], remainDmg);
                //if (Mathf.Abs(remainDmg - 0) < Mathf.Epsilon || Mathf.Abs(damageMake - 0) < Mathf.Epsilon)
                //{
                //    Debug.Log("");
                //}
                hitData[0].APawn.TakeDamage(damageMake, BattleDef.DAMAGE_TYPE.BULLET_PENETRATE);
                RealRange = Vector3.Distance(_SyncStartPos, hitData[0].HitPoints[0]);
            }
            return true;
        }
    }
}
