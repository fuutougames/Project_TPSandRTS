using Battle.Projectiles;
using UnityEngine;
using UnityEngine.Networking;

namespace Battle.Data
{
    public class CharacterBattleData : NetworkBase
    {
        #region Sync Vars
        [SyncVar(hook = "OnHPChanged")] private float _SyncHP;
        [SyncVar(hook = "OnPlayerLiveStateChanged")] private bool _SyncIsDead;
        #endregion

        private CapsuleCollider _CCollider;

        public CapsuleCollider CCollider
        {
            get
            {
                if (_CCollider == null)
                    _CCollider = gameObject.GetComponent<CapsuleCollider>();
                return _CCollider;
            }
        }

        [Client]
        private void OnHPChanged(float hp)
        {
            _SyncHP = hp;
            // send info maybe?
        }

        [Client]
        private void OnPlayerLiveStateChanged(bool isDead)
        {
            if (!_SyncIsDead || isDead)
            {
#if _DEBUG
                Debug.Log("Player " + GetInstanceID() + " is Dead!!!");
#endif
            }
            _SyncIsDead = isDead;
        }

        [ServerCallback]
        public void TakeDamage(float dmg, BattleDef.DAMAGE_TYPE dmgType)
        {
#if _DEBUG
            //Debug.Log("Damage Taken!!!");
#endif

            float hp = _SyncHP - dmg;
            if (hp < 0)
            {
                hp = 0;
                _SyncIsDead = true;
            }
            _SyncHP = hp;
        }


        public virtual void OnProjectileCollide(ProjectileBase projectile, Vector3 hitPoint,
                BattleDef.PROJECTILE_HITTYPE hitType, BattleDef.PROJECTILE_TYPE pType)
        {

        }
    }
}
