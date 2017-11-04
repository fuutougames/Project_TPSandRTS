using Battle.Projectiles;
using UnityEngine;
using UnityEngine.Networking;

namespace Battle.Data
{
    //[RequireComponent(typeof(PawnRuntimeData))]
    public class Pawn : MonoBase
    {
        private int _PawnID;
        public int PawnID { get { return _PawnID; } }

        //private PawnPreloadData m_PreloadData;
        //private PawnRuntimeData m_RuntimeData;
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

        //[Server]
        /// <summary>
        /// not calculate damage here, only take the result and update data
        /// </summary>
        /// <param name="dmg">damage taken</param>
        /// <param name="dmgType">damage type</param>
        public void TakeDamage(float dmg, BattleDef.DAMAGE_TYPE dmgType, Vector3[] hitPoints)
        {
            Debug.Log("Taking Dmg: " + dmg);
        }


        public virtual void OnProjectileCollide(ProjectileBase projectile, Vector3 hitPoint,
                BattleDef.PROJECTILE_HITTYPE hitType, BattleDef.PROJECTILE_TYPE pType)
        {
            Debug.Log("Pawn Hit!!!");
        }
    }
}

