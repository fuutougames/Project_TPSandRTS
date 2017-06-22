using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Const;

namespace Data.Config
{
    [System.Serializable]
    public class RangeDict : SerializableDictionaryBase<CommEnum.CHARACTER_ATTRIBS, Vector2> { };
    [CreateAssetMenu(fileName = "CharacterConf", menuName = "Config/CharacterConf", order = 1)]
    public class CharacterConf : ScriptableObject
    {
        /// <summary>
        /// 初始属性随机范围
        /// </summary>
        public RangeDict LowerBoundRndRange;
        /// <summary>
        /// 最大属性随机范围
        /// </summary>
        public RangeDict UpperBoundRndRange;
    }
}
