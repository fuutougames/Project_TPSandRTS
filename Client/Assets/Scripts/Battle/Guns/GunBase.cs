using System.Collections;
using System.Collections.Generic;
using Battle.Data;
using UnityEngine;

namespace Battle.Guns
{
    using Projectiles;

    public class GunBase : NetworkBase
    {
        [SerializeField] public Transform MuzzleTrans;
        [SerializeField] public List<Transform> AttachmentSlots;

        /// <summary>
        /// projectile template
        /// </summary>
        [SerializeField] private LinearProjectile _Projectile;

        private GunBattleData _GBData;

        private ProjectileBattleData _PBData = new ProjectileBattleData()
        {
            BaseDamage = 1000,
            MaxRange = 2000,
            Velocity = 300,
            Penetration = 100,
            PType = BattleDef.PROJECTILE_TYPE.LINEAR
        };
        /// <summary>
        /// 1 / _FireRate
        /// </summary>
        private float _FireInterval;


        private float _LastFireTime;

        public int CurrentRounds { get; private set; }
        public int TotalRounds { get; private set; }

        public bool _IsFiring;


        public virtual void Init(GunBattleData data)
        {
            _GBData = data;
            if (data.FireRate - .0f < Mathf.Epsilon)
                _FireInterval = float.MaxValue;
            else
                _FireInterval = 1.0f/data.FireRate;
        }

        public virtual void OnFire()
        {
            
        }

        /// <summary>
        /// single shot
        /// </summary>
        public void Fire()
        {
            //base.Fire();
            float now = TimeMgr.Instance.GetCurrentTime();
            if (now - _LastFireTime < _FireInterval)
                return;

            _LastFireTime = now;
            // fire
            // TODO: Calculate fire direction by accuracy value
            // temporary projectile creation, there should be a better way to instantiate a projectile

            float maxOffset = (float)2*1/_GBData.Accuracy;
            float offset = maxOffset * (float)GlobalInstances.Instance.RndIns.norm();

            ProjectileBase projectile = CreateProjectile();
            // temporary trigger
            projectile.Init(_PBData);
            // calculate it with angle instead of xy offset;
            Vector3 direction = (MuzzleTrans.forward + MuzzleTrans.right*offset + MuzzleTrans.up*offset).normalized;
            projectile.TriggerProjectile(MuzzleTrans.position, direction);
            OnFire();
        }

        private ProjectileBase CreateProjectile()
        {
            return GameObject.Instantiate(_Projectile);
        }

        public virtual void Reload()
        {

        }

        void FixedUpdate()
        {
            if (_IsFiring)
            {
                Fire();
            }
        }

    }
}
