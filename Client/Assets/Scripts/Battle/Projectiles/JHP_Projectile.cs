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
            return base.CalculateDamage(hitData, remainDmg);
        }

        public override bool ProcessHitData(List<PawnHitData> hitData, out int hitCnt)
        {
            hitCnt = 0;
            if (hitData.Count > 0)
            {
                hitCnt = 1;
                float remainDmg = _DmgLine.GetRemainDmgByTime(TimeMgr.Instance.GetCurrentTime());
                float damageMake = CalculateDamage(hitData[0], remainDmg);
                hitData[0].APawn.TakeDamage(damageMake, BattleDef.DAMAGE_TYPE.BULLET_PENETRATE);
            }
            return true;
        }
    }
}
