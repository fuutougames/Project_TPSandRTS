using System;
using System.Collections.Generic;
using Battle.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace Battle.Guns
{
    using Projectiles;

    public class GunBase : MonoBase
    {
        [SerializeField] public Transform MuzzleTrans;
        [SerializeField] public List<Transform> AttachmentSlots;

        /// <summary>
        /// projectile template
        /// </summary>
        [SerializeField] private BattleDef.PROJECTILE_TYPE _ProjectileType;

        private GunBattleData _GBData;
        private Pawn _Owner;

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

        /// <summary>
        /// server only value, not valid in client
        /// </summary>
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
        //[Server]
        public void Fire()
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
                side = _Owner.Side;

            projectile.TriggerProjectile(MuzzleTrans.position, direction, side);
            OnFire();

            // if is client, request fire then
            //RpcFireOnClient(MuzzleTrans.position, direction);
        }
        
        //[ClientRpc]
        /// <summary>
        /// TODO: Receive fire request from client
        /// </summary>
        /// <param name="firePos"></param>
        /// <param name="fireDir"></param>
        //private void RpcFireOnClient(Vector3 firePos, Vector3 fireDir)
        //{
        //    ProjectileBase projectile = CreateProjectile();
        //    projectile.Init(_PBData);
        //    int side = 0;
        //    if (_Owner != null)
        //        side = _Owner.Side;

        //    projectile.TriggerProjectile(firePos, fireDir, side);
        //    OnFire();
        //}

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

        protected override void OnFixedUpdate()
        {
            if (_IsFiring)
            {
                Fire();
            }
        }
    }
}
