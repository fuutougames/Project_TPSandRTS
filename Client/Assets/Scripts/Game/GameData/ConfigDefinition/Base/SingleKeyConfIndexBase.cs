using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Config
{
    public class SingleKeyConfIndexBase<TKey, TValue> : ScriptableObjectBase
    {
        public static Dictionary<TKey, TValue> _Dict;

        [UnityEngine.SerializeField]
        private TValue[] _Confs;

    }
}
