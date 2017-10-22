using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using Common;
using System;

namespace Battle.Data
{
    /// <summary>
    /// upload data to server, after that, client probably will never use this instance again
    /// preload data got everything that battle calculator need;
    /// </summary>
    public class PawnPreloadData : MonoBase
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

    }
}
