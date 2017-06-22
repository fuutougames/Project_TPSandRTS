using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.DataIns
{
    using Data.Const;
    public class CharacterAttribDictionary : SerializableDictionaryBase<CommEnum.CHARACTER_ATTRIBS, float> {};
    public class CharacterExpDictionary : SerializableDictionaryBase<CommEnum.GUN_TYPE, float> { };
    public class CharacterData
    {
        /// <summary>
        /// 角色基础属性数据
        /// </summary>
        public CharacterAttribDictionary AttribData;

        /// <summary>
        /// 角色装备熟练度数据
        /// </summary>
        public CharacterExpDictionary ExpData;

        /// <summary>
        /// construct function
        /// </summary>
        public CharacterData()
        {

        }

        /// <summary>
        /// class copy function
        /// </summary>
        /// <param name="data"></param>
        public CharacterData(CharacterData data)
        {

        }
    }
}