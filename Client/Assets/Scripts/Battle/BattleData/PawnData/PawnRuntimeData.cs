using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Battle.Data
{
    using DataPacks;
    public class PawnRuntimeData : NetworkBase
    {
        /// <summary>
        /// Unique id for pawn, generate by server
        /// </summary>
        [SyncVar(hook = "OnHPChanged")] private float _SyncHP;
        [SyncVar(hook = "OnMoveSpdChanged")] private float _SyncMoveSpd;
        [SyncVar(hook = "OnArmorChanged")] private float _SyncArmor;

        #region Getters and Setters
        public NetworkInstanceId PawnId { get { return Identity.netId; } }
        public float HP { get { return _SyncHP; } set { _SyncHP = value; } }
        public float MoveSpd { get { return _SyncMoveSpd; } set { _SyncMoveSpd = value; } }
        public float Armor { get { return _SyncArmor; } set { _SyncArmor = value; } }
        #endregion

        [Server]
        public void Init(PawnDataPack datapack)
        {
            _SyncHP = datapack.Hp;
            _SyncMoveSpd = datapack.MoveSpd;
            _SyncArmor = datapack.Armor;
        }

        #region Realtime Sync Vars Change Callbacks
        [Client]
        private void OnHPChanged(float hp)
        {
            _SyncHP = hp;
            // send info maybe?
        }

        [Client]
        private void OnMoveSpdChanged(float moveSpd)
        {
            _SyncMoveSpd = moveSpd;
        }

        [Client]
        private void OnArmorChanged(float armor)
        {
            _SyncArmor = armor;
        }
        #endregion
    }
}
