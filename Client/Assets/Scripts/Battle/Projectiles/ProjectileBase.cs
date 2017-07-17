using System;
using Battle;
using Data.Const;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Battle.Projectiles
{
    using Data;
    /// <summary>
    /// ProjectileBase
    /// Start position, direction, real range and state synchronize;
    /// </summary>
    public class ProjectileBase : NetworkBase, IMonoPoolItem
    {
        #region Sync Var

        [SyncVar(hook = "OnProjectileTrigger")] protected bool _SyncIsTriggered = false;
        [SyncVar(hook = "OnProjectileStartPosSet")] protected Vector3 _SyncStartPos;
        [SyncVar(hook = "OnProjectileDirectionSet")] protected Vector3 _SyncDirection;
        [SyncVar(hook = "OnProjectileTerminated")] protected bool _Terminated;
        //[SyncVar] protected float _SyncRealRange;

        #endregion

        #region Getters
        public Vector3 StartPos { get { return _SyncStartPos; } }
        #endregion


        #region Inner Var

        protected bool _IsFirstFrameIgnored = false;
        protected float _RealRange;
        [SerializeField]
        protected BattleDef.PROJECTILE_TYPE _ProjectileType;

        public BattleDef.PROJECTILE_TYPE ProjectileType
        {
            get { return _ProjectileType; }
        }

        [SerializeField]
        private TrailRenderer _TrailRenderer;

        #endregion

        #region Client Server Related

        /// <summary>
        /// Default Projectile Trigger Function;
        /// </summary>
        [ServerCallback]
        public void TriggerProjectile(Vector3 startPos, Vector3 direction)
        {
            _SyncStartPos = startPos;
            _SyncDirection = direction;
            // must call after pos and dir set;
            BeforeProjectileTrigger();
            _SyncIsTriggered = true;
            _IsFirstFrameIgnored = false;
            _Terminated = false;
            OnProjectileTrigger();
            RegisterProjectile();
        }



        [Client]
        protected void OnProjectileTrigger(bool isTrigger)
        {
            bool oldVal = _SyncIsTriggered;
            _SyncIsTriggered = isTrigger;
            if (!oldVal)
                _IsFirstFrameIgnored = false;
        }

        [Client]
        protected void OnProjectileStartPosSet(Vector3 startPos)
        {
            _SyncStartPos = startPos;
            CachedTransform.position = _SyncStartPos;
        }

        [Client]
        protected void OnProjectileDirectionSet(Vector3 direction)
        {
            _SyncDirection = direction;
            CachedTransform.forward = _SyncDirection;
        }

        [Client]
        protected void OnProjectileTerminated(bool terminated)
        {
            _Terminated = terminated;
            if (_Terminated)
            {
                // client terminate this projectile
            }
        }

        #endregion


        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (!_SyncIsTriggered || !_IsFirstFrameIgnored)
                return;

            UpdatePosition();
        }

        protected override void OnLateUpdate()
        {
            base.OnLateUpdate();
            if (_SyncIsTriggered && !_IsFirstFrameIgnored)
            {
                CachedTransform.position = _SyncStartPos;
                _IsFirstFrameIgnored = true;
            }
        }

        /// <summary>
        /// normal init
        /// </summary>
        public virtual void Init(ProjectileBattleData data)
        {

        }

        /// <summary>
        /// Position Update Function
        /// </summary>
        protected virtual void UpdatePosition()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void BeforeProjectileTrigger()
        {

        }

        protected virtual void OnProjectileTrigger()
        {
            if (_TrailRenderer != null)
                _TrailRenderer.enabled = true;
        }

        protected virtual void AfterProjectileTrigger()
        {

        }

        #region Show Effect Interfaces

        /// <summary>
        /// both server and client will run this function 
        /// when the collide occur, this function will call with collide information
        /// </summary>
        public virtual void OnStaticObstacleCollide(ObstacleData data, Vector3 hitPoint,
            BattleDef.PROJECTILE_HITTYPE hitType, BattleDef.PROJECTILE_TYPE pType)
        {
#if UNITY_EDITOR
            //Debug.Log("<color=cyan>Collide: "+node.Obstacle.transform.GetInstanceID()+"</color>");
#endif
        }


        /// <summary>
        /// will be called when collide an dynamic obstacle
        /// </summary>
        public virtual void OnDynamicObstacleCollide(DynamicObstacleData data, Vector3 hitPoint,
            BattleDef.PROJECTILE_HITTYPE hitType, BattleDef.PROJECTILE_TYPE pType)
        {

        }

        /// <summary>
        /// will be called when collide an character
        /// used for effect playing
        /// </summary>
        /// <param name="data"></param>
        /// <param name="hitPoint"></param>
        /// <param name="ctype"></param>
        public virtual void OnCharacterCollide(CharacterBattleData data, Vector3 hitPoint,
            BattleDef.PROJECTILE_HITTYPE hitType, BattleDef.PROJECTILE_TYPE pType)
        {

        }
        #endregion

        #region Collide Judge interfaces
        /// <summary>
        /// Client will need this function too
        /// </summary>
        /// <param name="time"></param>
        /// <param name="characterCollider"></param>
        /// <returns>Hit Point</returns>
        public virtual BattleDef.PROJECTILE_HITTYPE IsCollideWithCharacter(float time, CharacterBattleData cBData,
            out Vector3[] hitPoints, out float penLen)
        {
            hitPoints = null;
            penLen = .0f;
            return BattleDef.PROJECTILE_HITTYPE.NONE;
        }

        public virtual bool IsCollideWithDynamicObstacle(float time, DynamicObstacleData doData, out Vector3 hitPoint)
        {
            hitPoint = Vector3.zero;
            return false;
        }
        #endregion


        public virtual float CalculateDamage(Vector3 inPoint, CharacterBattleData character)
        {
            return 0;
        }

        /// <summary>
        /// process hit data, like damage calculation, damage line update, etc...
        /// </summary>
        /// <param name="hitData">hit data list</param>
        /// <param name="hitCnt">return how many character hit data is actually valid</param>
        /// <returns>is projectile life end</returns>
        public virtual bool ProcessHitData(List<CharacterHitData> hitData, out int hitCnt)
        {
            hitCnt = 0;
            return false;
        }


        public void RegisterProjectile()
        {
            BattleMgr.Instance.BData.RegisterProjectile(this);
        }

        public void UnRegisterProjectile()
        {
            BattleMgr.Instance.BData.UnRegisterProjectile(this);
        }

        /// <summary>
        /// TODO: Poolize projectiles
        /// </summary>
        public void DisposeProjectile()
        {
            UnRegisterProjectile();
            //GameObject.DestroyImmediate(this.gameObject);
        }

        public void OnRealDispose()
        {
            MonoObjPool<ProjectileBase> pool = GlobalObjPools.Instance.GetProjectilePoolByType(_ProjectileType);
            if (pool == null)
            {
#if UNITY_EDITOR
                throw new Exception("Pool not Initialize!!!");
#endif
            }
            pool.Push(this);
        }


        #region Pool Item Interfaces
        public void OnGet()
        {
            //throw new System.NotImplementedException();
            // do nothing here;
        }

        public void OnRetrun()
        {
            //throw new System.NotImplementedException();
            if (_TrailRenderer != null)
            {
                _TrailRenderer.enabled = false;
            }

            CachedTransform.position = new Vector3(100000, 100000, 100000);
        }
        #endregion
    }
}