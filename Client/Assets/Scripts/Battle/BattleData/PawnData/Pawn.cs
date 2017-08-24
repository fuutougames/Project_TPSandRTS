using Battle.Projectiles;
using UnityEngine;
using UnityEngine.Networking;

namespace Battle.Data
{
    [RequireComponent(typeof(PawnRuntimeData))]
    public class Pawn : NetworkBase
    {
        private PawnPreloadData _PreloadData;
        private PawnRuntimeData _RuntimeData;
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

        [Server]
        public void TakeDamage(float dmg, BattleDef.DAMAGE_TYPE dmgType)
        {
#if _DEBUG
            //Debug.Log("Damage Taken!!!");
#endif
            if (!isServer)
                return;

            float hp = _RuntimeData.HP - dmg;
            if (hp < 0)
            {
                hp = 0;
                //_SyncIsDead = true;
            }
            _RuntimeData.HP = hp;
        }


        public virtual void OnProjectileCollide(ProjectileBase projectile, Vector3 hitPoint,
                BattleDef.PROJECTILE_HITTYPE hitType, BattleDef.PROJECTILE_TYPE pType)
        {

        }
    }
}

