using System.Collections.Generic;
using UnityEngine;

namespace Battle.Projectiles
{
    public class AP_Projectile : LinearProjectile
    {
        protected override float CalcDmgLost(StaticObstacleData obstacle, float penLen)
        {
            return Mathf.Pow((obstacle.Hardness / _PBData.Penetration), penLen) - 1;
        }

        public float CalculateDamage(PawnHitData hitData, float remainDmg)
        {
            float dmg;
            if (hitData.APawn.Data.Armor < _PBData.Penetration)
                dmg = hitData.PenetrationLen * BattleConst.AP_PAWN_PENETRATE_DMG_WEIGHT * remainDmg;
            else
                dmg = remainDmg * BattleConst.AP_NOT_PENETRATE_DMG_WEIGHT;

            return dmg;
        }

        public override bool ProcessHitData(List<PawnHitData> hitData, int hitDataLen, out int hitCnt)
        {
            hitCnt = 0;
            if (hitDataLen == 0 || hitData.Count < hitDataLen)
            {
                return false;
            }

            // if not penetrate
            if (_PBData.Penetration < hitData[0].APawn.Data.Armor)
            {
                float damage = _DmgLine.GetRemainDmgByCurMagnitude(hitData[0].HitDistance);
                damage = CalculateDamage(hitData[0], damage);
                RealRange = hitData[0].HitDistance;
                hitData[0].APawn.TakeDamage(damage, BattleDef.DAMAGE_TYPE.BULLET_IMPACT, hitData[0].HitPoints);
                return true;
            }

            float dmgLost = 0;
            float remainDmg = _DmgLine.GetRemainDmgByCurMagnitude(hitData[0].HitDistance);
            for (int i = 0; i < hitDataLen; ++i)
            {
                if (Mathf.Abs(hitData[i].HitDistance - float.MaxValue) <= Mathf.Epsilon)
                {
                    break;
                }

                ++hitCnt;

                // calculate damage to all hited characters;
                float damage = CalculateDamage(hitData[i], remainDmg);
                remainDmg -= damage;
                // if remain dmg is smaller than 0, that means damage calculated is bigger than we want,
                // then make damge back to the correct value;
                if (remainDmg <= 0)
                {
                    // bullet stay inside target pawn
                    damage += remainDmg;
                    dmgLost += damage;
                    // damage multiply
                    damage *= BattleConst.AP_PAWN_STAY_INSIDE_DMG_WEIGHT;
                }
                else
                {
                    // bullet penetrate target pawn
                    dmgLost += damage;
                }

                //if (isServer)
                hitData[i].APawn.TakeDamage(damage, BattleDef.DAMAGE_TYPE.BULLET_PENETRATE, hitData[i].HitPoints);

                if (remainDmg <= 0)
                {
                    //RealRange = Vector3.Distance(hitData[i].HitPoints[0], _SyncStartPos);
                    RealRange = hitData[i].HitDistance;
                    return true;
                }
            }

            if (remainDmg > 0)
            {
                RealRange = Vector3.Distance(_DmgLine.UpdateDmgLine(TimeMgr.Instance.GetCurrentTime(), dmgLost), _SyncStartPos);
            }
            return false;
        }
    }
}
