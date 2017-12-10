using Battle.Projectiles;
using UnityEngine;
using UnityEngine.Networking;

namespace Battle.Data
{
    public class Pawn : MonoBase
    {
        private int _PawnID;
        public int PawnID { get { return _PawnID; } }

        private PawnData _PawnData
             = new PawnData()
             {
                 Armor = 1.0f
             };
        public PawnData Data
        {
            get { return _PawnData; }
        }
        private Collider _PCollider;
        public Collider PCollider
        {
            get
            {
                if (_PCollider == null)
                    _PCollider = GetComponent<Collider>();
                return _PCollider;
            }
        }

        protected bool _SwitchingWeapon = false;
        protected WeaponInterface _CurWeapon = null;

        //[Server]
        /// <summary>
        /// not calculate damage here, only take the result and update data
        /// only be called by battle manager
        /// </summary>
        /// <param name="dmg">damage taken</param>
        /// <param name="dmgType">damage type</param>
        public void TakeDamage(float dmg, DAMAGE_TYPE dmgType, PawnHitData hitData)
        {
            Debug.Log("Taking Dmg: " + dmg);
        }


        public virtual void OnProjectileCollide(ProjectileBase projectile, Vector3 hitPoint,
                PROJECTILE_HITTYPE hitType, PROJECTILE_TYPE pType)
        {
            Debug.Log("Pawn Hit!!!");
        }

        #region Base Actions
        /// <summary>
        /// Only be called by server
        /// </summary>
        public virtual void StartAttack()
        {
            if (_CurWeapon != null && !_SwitchingWeapon)
                _CurWeapon.StartAttack();
        }

        public virtual void CancelAttack()
        {
            if (_CurWeapon != null && !_SwitchingWeapon)
                _CurWeapon.CancelAttack();
        }


        /// <summary>
        /// Move to target position, call by user input or network RPC
        /// </summary>
        /// <param name="target"></param>
        public virtual void MoveTo(Vector3 target)
        {

        }

        /// <summary>
        /// Turn to target direction, call by user input or network RPC
        /// </summary>
        /// <param name="target"></param>
        public virtual void TurnTo(Quaternion target)
        {

        }
        #endregion

        #region Other Pawn Action
        ///
        /// More action such as crouch can be extended,
        /// But we will just suspend it for now
        ///
        #endregion

        #region Network Synchronization
        /// <summary>
        /// Pawn Position Synchronize
        /// </summary>
        public void OnPositionChanged()
        {

        }

        /// <summary>
        /// Pawn Facing Synchronize
        /// </summary>
        public void OnFaceDirChanged()
        {

        }

        /// <summary>
        /// Client RPC for Attack
        /// </summary>
        public void OnRPCAttack()
        {

        }
        #endregion
    }
}

