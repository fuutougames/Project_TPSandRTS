using System.Collections.Generic;
using Battle.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace Battle
{
    public class BattleData
    {
        public BattleDef.BATTLE_TYPE BType;
        private List<CharacterBattleData> _CharacterList;
        private Dictionary<int, ProjectileBase> _ActiveProjectiles;
        private List<TrapBase> _ActiveTraps;

        #region API
        public void Reset(BattleDef.BATTLE_TYPE btype = BattleDef.BATTLE_TYPE.TYPE_1)
        {
            BType = btype;
            _CharacterList = new List<CharacterBattleData>();
            _ActiveProjectiles = new Dictionary<int, ProjectileBase>();
            _ActiveTraps = new List<TrapBase>();
        }
        #endregion

        #region Projectiles
        public void UpdateProjectiles()
        {
            Dictionary<int, ProjectileBase>.Enumerator iter = _ActiveProjectiles.GetEnumerator();
            List<int> projectilesNeedToRemove = new List<int>();
            while(iter.MoveNext())
            {
                ProjectileBase projectile = iter.Current.Value;
                bool collideOccur = false;
                for (int i = 0; i < _CharacterList.Count; ++i)
                {
                    Vector3 hitPoint;

                    if (projectile.IsCollideWithCharacter(TimeMgr.Instance.GetCurrentTime(), _CharacterList[i].CCollider,
                        out hitPoint))
                    {
                        collideOccur = true;

                        float dmg = projectile.CalculateDamage(hitPoint, _CharacterList[i]);
                        
                        // server damage summary, ServerCallback function
                        // only run on server
                        _CharacterList[i].TakeDamage(dmg, BattleDef.DAMAGE_TYPE.BULLET_PENETRATE);

                        // break or not ?
                        break;
                    }
                }
                if (collideOccur)
                    projectilesNeedToRemove.Add(iter.Current.Key);
            }

            for (int i = 0; i < projectilesNeedToRemove.Count; ++i)
                UnRegisterProjectile(projectilesNeedToRemove[i]);
        }

        public void RegisterProjectile(ProjectileBase projectile)
        {
            if (_ActiveProjectiles.ContainsKey(projectile.GetInstanceID()))
                return;

            _ActiveProjectiles.Add(projectile.GetInstanceID(), projectile);
        }

        public void UnRegisterProjectile(int instanceId)
        {
            _ActiveProjectiles.Remove(instanceId);
        }
        #endregion
    }
}

