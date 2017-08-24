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
    public class PawnPreloadData : NetworkBase
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

        [Client]
        public void UploadPreloadData(Dictionary<CommEnum.PAWN_ATTRIBS, float> _attribmap)
        {
            CmdStartUpload();
            _AttribMap = _attribmap;
            Dictionary<CommEnum.PAWN_ATTRIBS, float>.Enumerator iter = _AttribMap.GetEnumerator();
            while (iter.MoveNext())
            {
                CmdUploadAttribVals((int)iter.Current.Key, iter.Current.Value);
            }
            for (int i = 0; i < _EquipList.Count; ++i)
            {
                CmdUploadEquipList(_EquipList[i]);
            }
            for (int i = 0; i < _WeaponList.Count; ++i)
            {
                CmdUploadWeaponList(_WeaponList[i]);
            }
            CmdUploadDone();
        }

        [Command]
        private void CmdStartUpload()
        {
            _AttribMap.Clear();
            _EquipList.Clear();
            _WeaponList.Clear();
        }

        [Command]
        private void CmdUploadAttribVals(int attribId, float attribVal)
        {
            _AttribMap.Add((CommEnum.PAWN_ATTRIBS)attribId, attribVal);
        }

        [Command]
        private void CmdUploadEquipList(int equipId)
        {
            _EquipList.Add(equipId);
        }

        [Command]
        private void CmdUploadWeaponList(int weaponId)
        {
            _WeaponList.Add(weaponId);
        }

        [Command]
        private void CmdUploadDone()
        {

        }
    }
}
