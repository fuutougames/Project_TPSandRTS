using System;
using System.Collections;
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
    public class ProjectileBase : MonoBase, IMonoPoolItem
    {
        #region Sync Var
        // not gonna use HLAPI, use LLAPI instead

        //[SyncVar(hook = "OnProjectileTrigger")] protected bool _SyncIsTriggered = false;
        protected Vector3 _SyncStartPos;
        protected Vector3 _SyncDirection;
        //[SyncVar] protected float _SyncRealRange;

        #endregion

        #region Getters
        public Vector3 StartPos { get { return _SyncStartPos; } }
        #endregion


        #region Inner Var
        //private bool _IsFirstFrameIgnored = false;
        protected float _RealRange;
        [SerializeField]
        protected BattleDef.PROJECTILE_TYPE _ProjectileType;

        public BattleDef.PROJECTILE_TYPE ProjectileType
        {
            get { return _ProjectileType; }
        }

        [SerializeField]
        private TrailRenderer _TrailEffect;

        protected bool _Disposed = true;
        public bool Disposed { get { return _Disposed; } }

        // frame count after trigger;
        private int framecnt = 0;
        //private bool _DisposeLock = false;

        #endregion

        public void TriggerProjectile(Vector3 startPos, Vector3 direction)
        {
            _SyncStartPos = startPos;
            _SyncDirection = direction;
            // must call after pos and dir set;
            //_SyncIsTriggered = true;
#if UNITY_EDITOR
            //Debug.LogError("Instance: " + this.GetInstanceID() + " Enable Trail on frame " + framecnt);
#endif
            if (_TrailEffect != null)
                _TrailEffect.enabled = true;

            // Delay triggeration for one frame to make sure trail renderer render correctly;
            StartCoroutine(OnDelayTrigger());
        }

        private IEnumerator OnDelayTrigger()
        {
            yield return null;
            CachedTransform.position = _SyncStartPos;
            CachedTransform.forward = _SyncDirection;
            BeforeProjectileTrigger();
            RegisterProjectile();
            framecnt = 0;
            _Disposed = false;
        }


        protected override void OnUpdate()
        {
            base.OnUpdate();
            ++framecnt;
            if (_Disposed/* || !_IsFirstFrameIgnored*/)
                return;

            UpdatePosition();
        }

        protected override void OnLateUpdate()
        {
            base.OnLateUpdate();
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

        #region Collide Judge interfaces
        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <param name="cBData"></param>
        /// <param name="hitPoints"></param>
        /// <param name="penLen"></param>
        /// <returns></returns>
        public virtual BattleDef.PROJECTILE_HITTYPE IsCollideWithPawn(float time, Pawn cBData,
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


        public virtual float CalculateDamage(PawnHitData hitData, float remaniDmg)
        {
            return 0;
        }

        /// <summary>
        /// process hit data, like damage calculation, damage line update, etc...
        /// </summary>
        /// <param name="hitData">hit data list</param>
        /// <param name="hitCnt">return how many character hit data is actually valid</param>
        /// <returns>is projectile life end, true end, false not end</returns>
        public virtual bool ProcessHitData(List<PawnHitData> hitData, int hitDataLen, out int hitCnt)
        {
            hitCnt = 0;
            return false;
        }

        #region Registration and Unregistration
        public void RegisterProjectile()
        {
            BattleMgr.Instance.BData.RegisterProjectile(this);
        }

        public void UnRegisterProjectile()
        {
            BattleMgr.Instance.BData.UnRegisterProjectile(this);
        }
        #endregion

        /// <summary>
        /// TODO: Poolize projectiles
        /// </summary>
        public void DisposeProjectile()
        {
            if (_Disposed)
                return;
            UnRegisterProjectile();
            _Disposed = true;
            //delay execute to make sure trail renderer is render correctly
            StartCoroutine(OnDelayDispose());
        }

        private IEnumerator OnDelayDispose()
        {
            // all delay here is to make sure trail renderer can render trail correctly
            int curframe = framecnt;
            yield return new WaitUntil(() => framecnt >= curframe);
            if (_TrailEffect != null)
            {
                _TrailEffect.enabled = false;
            }
#if UNITY_EDITOR
            //Debug.LogError("Instance: " + this.GetInstanceID() + " Delay Dispose on frame " + framecnt);
#endif
            yield return new WaitUntil(() => framecnt > curframe + 3);
            // move it to a remote place, do not change it's active state
            // Tecnically speaking, this should be done in OnRetrn function, but we need to return
            // this projectile in next frame to make sure trail renderer won't be activated too early;
            CachedTransform.position = new Vector3(100000, 100000, 100000);
#if UNITY_EDITOR
            //Debug.LogError("Instance: " + this.GetInstanceID() + " Delay Move away on frame " + framecnt);
#endif


            yield return new WaitUntil(() => framecnt > curframe + 5);
            MonoObjPool<ProjectileBase> pool = GlobalObjPools.Instance.GetProjectilePoolByType(_ProjectileType);
            if (pool == null)
            {
#if UNITY_EDITOR
                throw new Exception("Pool not Initialize!!!");
#endif
            }
#if UNITY_EDITOR
            //Debug.LogError("Instance: " + this.GetInstanceID() + " Delay Return on frame " + framecnt);
#endif
            pool.Push(this);
        }

        #region Pool Item Interfaces
        public void OnGet()
        {

        }

        public void OnReturn()
        {

        }
        #endregion
    }
}
