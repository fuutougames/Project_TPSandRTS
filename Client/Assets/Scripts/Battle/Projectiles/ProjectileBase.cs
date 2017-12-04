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
        private int _SyncSide;
        public int SyncSide
        {
            get
            {
                return _SyncSide;
            }
        }

        #endregion

        #region Getters
        public Vector3 StartPos { get { return _SyncStartPos; } }
        #endregion


        #region Inner Var
        //private bool _IsFirstFrameIgnored = false;
        [SerializeField]
        protected PROJECTILE_TYPE _ProjectileType;

        public PROJECTILE_TYPE ProjectileType
        {
            get { return _ProjectileType; }
        }

        [SerializeField]
        private TrailRenderer _TrailEffect;

        protected bool _Disposed = true;
        public bool Disposed { get { return _Disposed; } }

        // frame count after trigger;
        private int framecnt = 0;
        private bool _PreCalculated = false;
        public bool PreCalculated { get { return _PreCalculated; } }
        //private bool _DisposeLock = false;


        #endregion

        public void TriggerProjectile(Vector3 startPos, Vector3 direction, int side)
        {
            _SyncStartPos = startPos;
            _SyncDirection = direction;

            _SyncSide = side;
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
            //BeforeProjectileTrigger();
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
        public virtual void PreCalculateOnFirstFrame()
        {
            _PreCalculated = true;
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
        public virtual PROJECTILE_HITTYPE IsCollideWithPawn(float time, Pawn cBData,
            out Vector3[] hitPoints, out float penLen)
        {
            hitPoints = null;
            penLen = .0f;
            return PROJECTILE_HITTYPE.NONE;
        }

        public virtual bool IsCollideWithDynamicObstacle(float time, DynamicObstacleData doData, out Vector3 hitPoint)
        {
            hitPoint = Vector3.zero;
            return false;
        }
        #endregion


        //public virtual float CalculateDamage(PawnHitData hitData, float remaniDmg)
        //{
        //    return 0;
        //}

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
            OnDelayDispose();
            /*
            StartCoroutine(OnDelayDispose());
            */
        }

        private void OnDelayDispose()
        {

            Timer timer1 = TimerMgr.Instance.GetTimer();
            timer1.CompleteAction = () =>
            {
                if (_TrailEffect != null)
                {
                    _TrailEffect.enabled = false;
                }
                TimerMgr.Instance.ReturnTimer(ref timer1);

                Timer timer2 = TimerMgr.Instance.GetTimer();
                timer2.CompleteAction = () =>
                {
                    CachedTransform.position = new Vector3(100000, 100000, 100000);
                    TimerMgr.Instance.ReturnTimer(ref timer2);
                    Timer timer3 = TimerMgr.Instance.GetTimer();
                    timer3.CompleteAction = () =>
                    {
                        MonoObjPool<ProjectileBase> pool = GlobalObjPools.Instance.GetProjectilePoolByType(_ProjectileType);
                        _PreCalculated = false;
                        if (pool == null)
                        {
#if UNITY_EDITOR
                            throw new Exception("Pool not Initialize!!!");
#endif
                        }
                        pool.Push(this);
                        TimerMgr.Instance.ReturnTimer(ref timer3);
                    };
                    timer3.Reset(0.05f);
                    timer3.Start();
                };
                timer2.Reset(0.05f);
                timer2.Start();
            };
            timer1.Reset(0.05f);
            timer1.Start();
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
