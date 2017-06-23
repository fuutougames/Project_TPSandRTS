using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Battle
{
    public class BattleCharacterData : NetworkBase
    {
        #region Sync Vars
        [SyncVar(hook = "OnHPChanged")] private float _SyncHP;
        #endregion

        public CapsuleCollider CCollider;

        [Client]
        private void OnHPChanged(float hp)
        {
            _SyncHP = hp;
            // send info maybe?
        }

        [Server]
        public void TakeDamage(float baseDmg, BattleDef.DAMAGE_TYPE dmgType)
        {

        }
    }
}
