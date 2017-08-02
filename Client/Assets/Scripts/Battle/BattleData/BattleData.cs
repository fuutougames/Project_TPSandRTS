using System.Collections.Generic;
using Battle.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace Battle
{
    using Projectiles;
    public class BattleData
    {
        public BattleDef.BATTLE_TYPE BType;
        private Dictionary<int, CharacterBattleData> _CharacterList;
        private Dictionary<int, ProjectileBase> _ActiveProjectiles;
        private List<TrapBase> _ActiveTraps;

        #region API
        public void Reset(BattleDef.BATTLE_TYPE btype = BattleDef.BATTLE_TYPE.TYPE_1)
        {
            BType = btype;
            _CharacterList = new Dictionary<int, CharacterBattleData>();
            _ActiveProjectiles = new Dictionary<int, ProjectileBase>();
            _ActiveTraps = new List<TrapBase>();
        }
        #endregion

        #region Projectiles
        private readonly List<CharacterHitData> _HitedCharacterList = new List<CharacterHitData>();
        private int _HitedCharacterCnt = 0;
        private bool _UpdatingProjectiles = false;
        private readonly List<ProjectileBase> _RegisterBuffer = new List<ProjectileBase>();
        private readonly List<ProjectileBase> _UnRegisterBuffer = new List<ProjectileBase>(); 
        public void UpdateProjectiles()
        {
            _UpdatingProjectiles = true;
            Dictionary<int, ProjectileBase>.Enumerator iter = _ActiveProjectiles.GetEnumerator();
            // List<int> projectilesNeedToRemove = new List<int>();
            // find all collided character, and do the damage calculation and damage line update later;
            while (iter.MoveNext())
            {
                if (iter.Current.Value.Disposed)
                    continue;

                _HitedCharacterCnt = 0;
                ProjectileBase projectile = iter.Current.Value;
                //bool collideOccur = false;
                Dictionary<int, CharacterBattleData>.Enumerator cIter = _CharacterList.GetEnumerator();
                while(cIter.MoveNext())
                { 
                    Vector3[] hitPoints;
                    float penLen;
                    // if hit character
                    BattleDef.PROJECTILE_HITTYPE hitType =
                        projectile.IsCollideWithCharacter(TimeMgr.Instance.GetCurrentTime(), cIter.Current.Value,
                            out hitPoints, out penLen);

                    // hit data calculation
                    // data use for damage calculation and display hit effect
                    if (hitType != BattleDef.PROJECTILE_HITTYPE.NONE)
                    {
                        //collideOccur = true;
                        CharacterHitData cHitData;
                        if (_HitedCharacterCnt >= _HitedCharacterList.Count)
                        {
                            cHitData = new CharacterHitData();
                            _HitedCharacterList.Add(cHitData);
                        }
                        else
                        {
                            cHitData = _HitedCharacterList[_HitedCharacterCnt];
                        }
                        cHitData.Character = cIter.Current.Value;
                        cHitData.PenetrationLen = penLen;
                        cHitData.HitPoints = hitPoints;
                        cHitData.HitType = hitType;
                        if (hitPoints.Length >= 1)
                        {
                            cHitData.HitDistance = (hitPoints[0] - projectile.StartPos).magnitude;
                        }

                        ++_HitedCharacterCnt;
                    }

                    // TODO: if hit dynamic obstacles
                    // this will have to update whole damage line
                }

                // move the useless hit data away
                if (_HitedCharacterCnt < _HitedCharacterList.Count)
                {
                    for (int i = _HitedCharacterCnt; i < _HitedCharacterList.Count; ++i)
                    {
                        _HitedCharacterList[i].HitDistance = float.MaxValue;
                    }
                }

                // TODO: sort the hit data list by hit distance
                _HitedCharacterList.Sort(CharacterHitData.SortByHitDistance);

                int hitCnt;
                // process hit data
                // TODO: implement process function for every projectile
                projectile.ProcessHitData(_HitedCharacterList, out hitCnt);

                // display hit effect
                for (int i = 0; i < hitCnt; ++i)
                {
                    CharacterHitData data = _HitedCharacterList[i];
                    if (data.HitType == BattleDef.PROJECTILE_HITTYPE.PENETRATE)
                    {
                        data.Character.OnProjectileCollide(projectile, data.HitPoints[0], BattleDef.PROJECTILE_HITTYPE.IN, 
                            projectile.ProjectileType);
                        data.Character.OnProjectileCollide(projectile, data.HitPoints[1], BattleDef.PROJECTILE_HITTYPE.OUT,
                            projectile.ProjectileType);
                    }
                    else
                    {
                        data.Character.OnProjectileCollide(projectile, data.HitPoints[0], data.HitType,
                            projectile.ProjectileType);
                    }
                }
            }

            // don't unregister it here, just set it's real range and let projectile dispose by itself; 
            // real range should be reset in ProcessHitData function
            // mind the [delete element in a loop] problem
            _UpdatingProjectiles = false;

            for (int i = 0; i < _RegisterBuffer.Count; ++i)
            {
                RegisterProjectile(_RegisterBuffer[i]);
            }
            _RegisterBuffer.Clear();
            for (int i = 0; i < _UnRegisterBuffer.Count; ++i)
            {
                UnRegisterProjectile(_UnRegisterBuffer[i]);
            }
            _UnRegisterBuffer.Clear();
        }

        public void RegisterProjectile(ProjectileBase projectile)
        {
            if (_UpdatingProjectiles)
            {
                _RegisterBuffer.Add(projectile);
                return;
            }

            if (_ActiveProjectiles.ContainsKey(projectile.GetInstanceID()))
                return;

            _ActiveProjectiles.Add(projectile.GetInstanceID(), projectile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceId">instacnce id of projectile script</param>
        public void UnRegisterProjectile(ProjectileBase projectile)
        {
            int instanceId = projectile.GetInstanceID();
            if (_UpdatingProjectiles && _ActiveProjectiles.ContainsKey(instanceId))
            {
                _UnRegisterBuffer.Add(projectile);
                return;
            }
            if (_ActiveProjectiles.ContainsKey(instanceId))
            {
                _ActiveProjectiles.Remove(instanceId);
            }
        }
        #endregion



        #region Characters
        public void RegisterCharacter(CharacterBattleData cbData)
        {
            if (_CharacterList.ContainsKey(cbData.GetInstanceID()))
                return;

            _CharacterList.Add(cbData.GetInstanceID(), cbData);
        }

        public void UnRegisterCharacter(int instanceId)
        {
            _CharacterList.Remove(instanceId);
        }
        #endregion
    }
}

