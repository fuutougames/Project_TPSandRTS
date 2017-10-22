using Battle.Projectiles;
using UnityEngine;
using UnityEngine.Networking;

namespace Battle.Data
{
    [RequireComponent(typeof(PawnRuntimeData))]
    public class Pawn : MonoBase
    {
        private PawnPreloadData m_PreloadData;
        private PawnRuntimeData m_RuntimeData;

        private Collider m_PCollider;
        public Collider PCollider
        {
            get
            {
                if (m_PCollider == null)
                    m_PCollider = GetComponent<Collider>();
                return m_PCollider;
            }
        }

        //[Server]
        public void TakeDamage(float dmg, BattleDef.DAMAGE_TYPE dmgType)
        {
#if _DEBUG
            //Debug.Log("Damage Taken!!!");
#endif
            //if (!isServer)
                //return;

            //float hp = m_RuntimeData.HP - dmg;
            //if (hp < 0)
            //{
            //    hp = 0;
            //    //_SyncIsDead = true;
            //}
            //m_RuntimeData.HP = hp;
        }


        public virtual void OnProjectileCollide(ProjectileBase projectile, Vector3 hitPoint,
                BattleDef.PROJECTILE_HITTYPE hitType, BattleDef.PROJECTILE_TYPE pType)
        {
            Debug.Log("Pawn Hit!!!");
        }
    }
}

