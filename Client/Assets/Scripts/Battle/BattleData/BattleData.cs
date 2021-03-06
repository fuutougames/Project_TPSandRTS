using System.Collections.Generic;
using Battle.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace Battle
{
    using Projectiles;
    using ContractLogics;
    public class BattleData
    {
        //public BattleDef.BATTLE_TYPE BType;
        //private ContractLogicBase _MainContract;
        public BATTLE_STATE BState;
        private Dictionary<int, Pawn> _PawnList;
        private Dictionary<int, ProjectileBase> _ActiveProjectiles;
        private List<TrapBase> _ActiveTraps;
        private Timer _BattleTimer;

        private BattleSceneData _SceneData;
        public BattleSceneData SceneData { get { return _SceneData; } }

        #region Battle Setting
        private bool _FriendlyFire = false;
        public bool FriendlyFire
        {
            get { return _FriendlyFire; }
        }
        private int m_MySide;
        public int MySide
        {
            get
            {
                return m_MySide;
            }
        }
        #endregion

        #region API
        public void NetworkInit()
        {

        }

        public void Reset(float timeLimit = -1)
        {
            //BType = btype;
            // start main loop by setting state as ONGOING
            //_MainContract = mainCommission;
            BState = BATTLE_STATE.ONGOING;
            _PawnList = new Dictionary<int, Pawn>();
            _ActiveProjectiles = new Dictionary<int, ProjectileBase>();
            _ActiveTraps = new List<TrapBase>();

            if (SceneData == null)
                _SceneData = new BattleSceneData();
            SceneData.Reset();

            if (timeLimit > 0)
            {
                _BattleTimer = TimerMgr.Instance.GetTimer();
                _BattleTimer.Reset(timeLimit);
                _BattleTimer.CompleteAction = OnBattleTimerEnd;
                _BattleTimer.UpdateAction = OnBattleTimerUpdate;
            }
        }

        public void EndBattle()
        {
            BState = BATTLE_STATE.END;
            // go back to base
            // and do battle settlement
        }

        private void OnBattleTimerUpdate(float reaminTime)
        {
            // update timer text on ui
        }

        private void OnBattleTimerEnd()
        {
            if (_BattleTimer != null)
                TimerMgr.Instance.ReturnTimer(ref _BattleTimer);

            EndBattle();
        }
        #endregion

        #region Projectiles
        private readonly List<PawnHitData> _HitedPawnList = new List<PawnHitData>();
        private int _HitedPawnCnt = 0;
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

                if (!iter.Current.Value.PreCalculated)
                    iter.Current.Value.PreCalculateOnFirstFrame();

                _HitedPawnCnt = 0;
                ProjectileBase projectile = iter.Current.Value;
                //bool collideOccur = false;
                Dictionary<int, Pawn>.Enumerator cIter = _PawnList.GetEnumerator();
                while(cIter.MoveNext())
                { 
                    Vector3[] hitPoints;
                    float penLen;
                    // if hit character
                    PROJECTILE_HITTYPE hitType =
                        projectile.IsCollideWithPawn(TimeMgr.Instance.GetCurrentTime(), cIter.Current.Value,
                            out hitPoints, out penLen);

                    // hit data calculation
                    // data use for damage calculation and display hit effect
                    if (hitType != PROJECTILE_HITTYPE.NONE)
                    {
                        //collideOccur = true;
                        PawnHitData cHitData;
                        if (_HitedPawnCnt >= _HitedPawnList.Count)
                        {
                            cHitData = new PawnHitData();
                            _HitedPawnList.Add(cHitData);
                        }
                        else
                        {
                            cHitData = _HitedPawnList[_HitedPawnCnt];
                        }
                        cHitData.APawn = cIter.Current.Value;
                        cHitData.PenetrationLen = penLen;
                        cHitData.HitPoints = hitPoints;
                        cHitData.HitType = hitType;
                        if (hitPoints.Length >= 1)
                        {
                            cHitData.HitDistance = (hitPoints[0] - projectile.StartPos).magnitude;
                        }

                        ++_HitedPawnCnt;
                    }

                    // TODO: if hit dynamic obstacles
                    // this will have to update whole damage line
                }

                // move the useless hit data away
                if (_HitedPawnCnt < _HitedPawnList.Count)
                {
                    for (int i = _HitedPawnCnt; i < _HitedPawnList.Count; ++i)
                    {
                        _HitedPawnList[i].HitDistance = float.MaxValue;
                    }
                }

                // TODO: sort the hit data list by hit distance
                _HitedPawnList.Sort(PawnHitData.SortByHitDistance);

                int hitCnt;
                // process hit data
                // TODO: implement process function for every projectile
                projectile.ProcessHitData(_HitedPawnList, _HitedPawnCnt, out hitCnt);

                // display hit effect
                for (int i = 0; i < hitCnt; ++i)
                {
                    PawnHitData data = _HitedPawnList[i];
                    if (data.HitType == PROJECTILE_HITTYPE.PENETRATE)
                    {
                        data.APawn.OnProjectileCollide(projectile, data.HitPoints[0], PROJECTILE_HITTYPE.IN, 
                            projectile.ProjectileType);
                        data.APawn.OnProjectileCollide(projectile, data.HitPoints[1], PROJECTILE_HITTYPE.OUT,
                            projectile.ProjectileType);
                    }
                    else
                    {
                        data.APawn.OnProjectileCollide(projectile, data.HitPoints[0], data.HitType,
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

        #region Pawns
        public void RegisterPawn(Pawn cbData)
        {
            if (_PawnList.ContainsKey(cbData.PawnID))
                return;

            _PawnList.Add(cbData.PawnID, cbData);
        }

        public void UnRegisterPawn(int pawnId)
        {
            _PawnList.Remove(pawnId);
        }
        #endregion

        #region Update
        public void Update()
        {
            if (BState != BATTLE_STATE.ONGOING)
                return;

            // is player controlled character all dead

            // about contracts
            // we decide to update contract via battle events instead of update it every frame

            UpdateProjectiles();
        }
        #endregion
    }
}

