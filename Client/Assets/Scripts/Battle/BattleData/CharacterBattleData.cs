using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Battle
{
    public class CharacterBattleData : NetworkBase
    {
        #region Sync Vars
        [SyncVar(hook = "OnHPChanged")] private float _SyncHP;
        [SyncVar(hook = "OnPlayerLiveStateChanged")] private bool _SyncIsDead;
        #endregion

        public CapsuleCollider CCollider;

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
            Debug.Log("Damage Taken!!!");
#endif

            float hp = _SyncHP - dmg;
            if (hp < 0)
            {
                hp = 0;
                _SyncIsDead = true;
            }
            _SyncHP = hp;
        }
    }
}
