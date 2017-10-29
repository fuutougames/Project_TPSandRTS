using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Battle.Data
{
    public class PawnData 
    {
        private Dictionary<CommEnum.PAWN_ATTRIBS, float> _AttribMap = new Dictionary<CommEnum.PAWN_ATTRIBS, float>(8);
        private List<int> _EquipList = new List<int>();
        private List<int> _WeaponList = new List<int>();

        public float GetAttribVal(CommEnum.PAWN_ATTRIBS attribType)
        {
            float val = .0f;
            _AttribMap.TryGetValue(attribType, out val);
            return val;
        }

        private Dictionary<int, float> _ExpMap = new Dictionary<int, float>();



        /*[SyncVar(hook = "OnHPChanged")]*/
        private float _SyncHP;
        /*[SyncVar(hook = "OnMoveSpdChanged")]*/
        private float _SyncMoveSpd;
        /*[SyncVar(hook = "OnArmorChanged")]*/
        private float _SyncArmor;

        #region Getters and Setters
        //public NetworkInstanceId PawnId { get { return Identity.netId; } }
        public float HP { get { return _SyncHP; } set { _SyncHP = value; } }
        public float MoveSpd { get { return _SyncMoveSpd; } set { _SyncMoveSpd = value; } }
        public float Armor { get { return _SyncArmor; } set { _SyncArmor = value; } }
        #endregion

        //[Server]
    }
}
