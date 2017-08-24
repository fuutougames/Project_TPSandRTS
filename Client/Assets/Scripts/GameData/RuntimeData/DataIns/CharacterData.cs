using System.Collections.Generic;
using Common;

namespace Data.DataIns
{
    public class CharacterAttribDictionary : SerializableDictionaryBase<CommEnum.PAWN_ATTRIBS, float> {};
    public class CharacterExpDictionary : SerializableDictionaryBase<CommEnum.GUN_TYPE, float> { };
    public class PawnData
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
        public PawnData()
        {

        }

        /// <summary>
        /// class copy function
        /// </summary>
        /// <param name="data"></param>
        public PawnData(PawnData data)
        {

        }
    }
}
