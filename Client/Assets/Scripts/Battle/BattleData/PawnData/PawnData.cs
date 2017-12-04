using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using Network;

namespace Battle.Data
{
    public class PawnData 
    {
        private int _PawnID;
        public int PawnID { get { return _PawnID; } }

        #region Static Data
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
        #endregion



        #region Dynamic Data
        private float _SyncHP;
        private float _SyncMoveSpd;
        private int _SyncSide;
        private float _SyncArmor;
        private Dictionary<int, int> _Ammo = new Dictionary<int, int>();

        #region Getters and Setters
        public float HP { get { return _SyncHP; } set { _SyncHP = OnHpChange(value); } }
        public float MoveSpd { get { return _SyncMoveSpd; } set { _SyncMoveSpd = OnMoveSpdChange(value); } }
        public float Armor { get { return _SyncArmor; } set { _SyncArmor = OnArmorChange(value); } }
        public int Side { get { return _SyncSide; } set { _SyncSide = OnSideChange(value); } }


        private float OnHpChange(float newVal)
        {
            if (NetworkMgr.Instance.TerminalType == Terminal.TERMINAL_TYPE.SERVER)
            {
                return newVal;
            }
            return HP;
        }

        private float OnMoveSpdChange(float newVal)
        {
            if (NetworkMgr.Instance.TerminalType == Terminal.TERMINAL_TYPE.SERVER)
            {
                return newVal;
            }
            return MoveSpd;
        }

        private float OnArmorChange(float newVal)
        {
            if (NetworkMgr.Instance.TerminalType == Terminal.TERMINAL_TYPE.SERVER)
            {
                return newVal;
            }
            return Armor;
        }

        private int OnSideChange(int newVal)
        {
            if (NetworkMgr.Instance.TerminalType == Terminal.TERMINAL_TYPE.SERVER)
            {
                return newVal;
            }
            return Side;
        }

        #endregion
        #endregion
    }
}
