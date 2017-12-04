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

        public int RemainRounds { get; private set; }

        #region Fire Control Attributes
        /// <summary>
        /// 1 / _FireRate
        /// </summary>
        private float _FireInterval;
        private float _LastFireTime;
        private FIRE_MODE _FireMode;
        private bool _Reloading = false;

        /// <summary>
        /// server only value, not valid in client
        /// </summary>
        public bool _IsShooting;
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
        /// single shot
        /// </summary>
        //[Server]
        public void Shoot()
        {
            //base.Fire();
            // if is server, fire
            float now = TimeMgr.Instance.GetCurrentTime();
            if (now - _LastFireTime < _FireInterval)
                return;

            _LastFireTime = now;
            // fire
            // TODO: Calculate maxOffset by accuracy value
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
            if (_IsShooting)
            {
                Shoot();
            }
        }

        public void Attack()
        {
            throw new NotImplementedException();
        }
    }
}
