using System.Collections.Generic;
using UnityEngine;

namespace Battle.Projectiles
{
    public class AP_Projectile : LinearProjectile
    {
        protected override float CalcDmgLost(StaticObstacleData obstacle, float penLen)
        {
            return base.CalcDmgLost(obstacle, penLen);
        }

        public override float CalculateDamage(CharacterHitData hitData, float remainDmg)
        {
            return base.CalculateDamage(hitData, remainDmg);
        }

        public override bool ProcessHitData(List<CharacterHitData> hitData, out int hitCnt)
        {
            hitCnt = 0;
            float dmgLost = 0;
            float remainDmg = _DmgLine.GetRemainDmgByTime(TimeMgr.Instance.GetCurrentTime());
            for (int i = 0; i < hitData.Count; ++i)
            {
                if (Mathf.Abs(hitData[i].HitDistance - float.MaxValue) <= Mathf.Epsilon)
                {
                    break;
                }
                ++hitCnt;

                // calculate damage to all hited characters;
                float damage = 0;
                remainDmg -= damage;
                if (remainDmg <= 0)
                    damage += remainDmg;
                dmgLost += damage;

                //if (isServer)
                hitData[i].Character.TakeDamage(damage, BattleDef.DAMAGE_TYPE.BULLET_PENETRATE);

                if (remainDmg <= 0)
                {
                    _RealRange = Vector3.Distance(hitData[i].HitPoints[0], _SyncStartPos);
                    return true;
                }
            }

            if (remainDmg > 0)
            {
                _RealRange = Vector3.Distance(_DmgLine.UpdateDmgLine(TimeMgr.Instance.GetCurrentTime(), dmgLost), _SyncStartPos);
            }
            return false;
        }
    }
}