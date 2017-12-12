using System;
using System.Collections.Generic;
using Battle.Data;
using UnityEngine;

namespace Battle.Guns
{
    using Projectiles;

    public class GunBase : MonoBase, WeaponInterface
    {
        [SerializeField] public Transform MuzzleTrans;
        [SerializeField] public List<Transform> AttachmentSlots;

        /// <summary>
        /// projectile template
        /// </summary>
        [SerializeField] private PROJECTILE_TYPE _ProjectileType;

        private Pawn _Owner;

        private GunBattleData _GBData;

        #region Projectile Attributes
        private ProjectileBattleData _PBData = new ProjectileBattleData()
        {
            BaseDamage = 1000,
            MaxRange = 2000,
            Velocity = 300,
            Penetration = 100,
            PType = PROJECTILE_TYPE.JHP_PROJECTILE
        };
        #endregion

        #region Mag Data

        public int MagCapacity;
        public int RemainRounds { get; private set; }
        #endregion

        #region Fire Control Attributes
        /// <summary>
        /// 1 / _FireRate
        /// </summary>
        private float _FireInterval;
        private float _LastFireTime;
        private int _BurstCount = 3;
        //private int _ShotsRemainingInBurst;
        private FIRE_MODE _FireMode;
        private bool _Reloading = false;

        /// <summary>
        /// server only value, not valid in client
        /// </summary>
        public bool _IsAttacking;
        #endregion


        public virtual void Init(GunBattleData data)
        {
            _GBData = data;
            if (data.FireRate - .0f < Mathf.Epsilon)
                _FireInterval = float.MaxValue;
            else
                _FireInterval = 1.0f/data.FireRate;
        }

        public virtual void OnShoot()
        {
            
        }

        /// <summary>
        /// single basic shot
        /// </summary>
        public void BasicFire()
        {
            // TODO: Calculate maxOffset by accuracy value
            --RemainRounds;
            _LastFireTime = TimeMgr.Instance.GetCurrentTime();
            float maxOffset = (float)2*1/_GBData.Accuracy;
            float offset = maxOffset * (float)GlobalInstances.Instance.RndIns.norm();
            float maxHalfOffsetAngle = 45;
            float realOffsetAngle = maxHalfOffsetAngle*offset;
            float rotate = 360*(float) GlobalInstances.Instance.RndIns.norm();
            Quaternion quatOffset = Quaternion.AngleAxis(realOffsetAngle, MuzzleTrans.right);
            Quaternion quatRotate = Quaternion.AngleAxis(rotate, MuzzleTrans.forward);
            Vector3 direction = quatRotate*quatOffset*MuzzleTrans.forward;
            ProjectileBase projectile = CreateProjectile();
            // temporary trigger
            projectile.Init(_PBData);
            // calculate it with angle instead of xy offset;
            int side = 0;
            if (_Owner != null)
                side = _Owner.Data.Side;

            projectile.TriggerProjectile(MuzzleTrans.position, direction, side);
            OnShoot();
        }
        
        private ProjectileBase CreateProjectile()
        {
            // temporary projectile creation, there should be a better way to instantiate a projectile
            // TODO: Poolize peojectile;
            //return GameObject.Instantiate(_Projectile);
            MonoObjPool<ProjectileBase> pool = GlobalObjPools.Instance.GetProjectilePoolByType(_ProjectileType);
            if (pool == null)
            {
#if UNITY_EDITOR
                throw new Exception("Pool not Initialize!!!");
#endif
            }
            return pool.Pop();
        }

        public virtual void Reload()
        {
            RemainRounds = MagCapacity;
        }

        public virtual void SetFireMode(FIRE_MODE fireMode)
        {
            _FireMode = fireMode;
        }

        /// <summary>
        /// aim target
        /// </summary>
        /// <param name="target"></param>
        public virtual void Aim(Vector3 target)
        {

        }

        protected override void OnFixedUpdate()
        {
            Attack();
        }

        private void AutoFire()
        {
            float now = TimeMgr.Instance.GetCurrentTime();
            if (now - _LastFireTime < _FireInterval)
                return;

            BasicFire();
        }

        private bool _Bursting = false;
        private bool _BurstComplete = true;
        //private int _LastBurstCount = 0;
        private int _ShotsRemainingInBurst = 0;
        private void BurstFire()
        {
            //if (_Bursting)
            //    return;

            float now = TimeMgr.Instance.GetCurrentTime();
            if (now - _LastFireTime < _FireInterval)
                return;

            if (_ShotsRemainingInBurst > 0)
                return;

            _ShotsRemainingInBurst = RemainRounds - _BurstCount > 0 ? _BurstCount : RemainRounds;
            //_LastBurstCount = _ShotsRemainingInBurst;

            _Bursting = true;
            _BurstComplete = false;
            Timer timer = TimerMgr.Instance.GetTimer();
            timer.CompleteAction = () =>
            {
                // in case the final round is not being shot at the update action
                if (_ShotsRemainingInBurst > 0)
                {
                    BasicFire();
                    --_ShotsRemainingInBurst;
                }
                _BurstComplete = true;
                TimerMgr.Instance.ReturnTimer(ref timer);
            };
            timer.UpdateAction = (remainTime) =>
            {
                if (remainTime < _FireInterval * _ShotsRemainingInBurst)
                {
                    BasicFire();
                    --_ShotsRemainingInBurst;
                }
            };
            timer.Reset(_FireInterval * _ShotsRemainingInBurst);
            timer.Start();
            BasicFire();
            --_ShotsRemainingInBurst;
        }

        private bool _SingleShooting = false;
        private void SingleFire()
        {
            //if (!_SingleShooting)
            //    return;

            _SingleShooting = true;
            float now = TimeMgr.Instance.GetCurrentTime();
            if (now - _LastFireTime < _FireInterval)
                return;

            BasicFire();
        }

        protected void Attack()
        {
            if (!_IsAttacking)
                return;
            if (_SingleShooting)
                return;
            if (_Bursting)
                return;
            if (!_BurstComplete)
                return;

            if (RemainRounds <= 0)
            {
                // do something maybe
                return;
            }

            switch (_FireMode)
            {

                case FIRE_MODE.AUTO:
                    AutoFire();
                    break;
                case FIRE_MODE.BURST:
                    BurstFire();
                    break;
                case FIRE_MODE.SINGLE:
                    SingleFire();
                    break;
            }
        }

        /// <summary>
        /// switch weapon state into attack
        /// </summary>
        public void StartAttack()
        {
            _IsAttacking = true;
        }

        /// <summary>
        /// switch weapon state into normal
        /// </summary>
        public void CancelAttack()
        {
            _IsAttacking = false;
            _SingleShooting = false;
            _Bursting = false;
        }
    }
}
