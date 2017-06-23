using System.Collections.Generic;
using Battle.Data;
using UnityEngine;

namespace Battle
{
    public class BattleData
    {
        public BattleDef.BATTLE_TYPE BType;
        private List<BattleCharacterData> _CharacterList;
        private List<ProjectileBase> _ActiveProjectiles;
        private List<TrapBase> _ActiveTraps;

        #region Projectiles
        public void UpdateProjectile()
        {
            for (int i = 0; i < _ActiveProjectiles.Count; ++i)
            {
                ProjectileBase projectile = _ActiveProjectiles[i];
                for (int j = 0; j < _CharacterList.Count; ++j)
                {
                    Vector3 hitPoint;
                    if (projectile.IsCollideWithCharacter(TimeMgr.Instance.GetCurrentTime(), _CharacterList[j].CCollider,
                        out hitPoint))
                    {
                        float dmg = projectile.CalculateDamage(hitPoint, _CharacterList[j]);
                        _CharacterList[j].TakeDamage(dmg, BattleDef.DAMAGE_TYPE.BULLET_PENETRATE);
                    }
                }
            }
        }

        public void RegisterProjectile(ProjectileBase projectile)
        {
            
        }

        public void UnRegisterProjectile(ProjectileBase projectile)
        {

        }
        #endregion
    }
}

