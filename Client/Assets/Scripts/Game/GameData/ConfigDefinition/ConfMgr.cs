using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using System;

namespace Config
{
    public class ConfMgr : Singleton<ConfMgr>
    {
        public const string CONFPATH_PREFIX = "ConfigData/";

        private Dictionary<Type, ScriptableObjectBase> _ConfIdxMap;

        private Dictionary<Type, string> _ConfPathMap;

        public ConfMgr()
        {
            _ConfIdxMap = new Dictionary<Type, ScriptableObjectBase>();

            _ConfPathMap = new Dictionary<Type, string>()
            {
                { typeof(ContractConfIndex), "ContractConfs/Index/Index" },
            };
        }


        public T GetConfIndex<T> () where T: ScriptableObjectBase
        {
            Type t = typeof(T);
            ScriptableObjectBase confIdx;
            if (!_ConfIdxMap.TryGetValue(t, out confIdx))
            {
                // try load config
                confIdx = Resources.Load<T>(CONFPATH_PREFIX + _ConfPathMap[t]);
                _ConfIdxMap.Add(t, confIdx);
            }
            return (confIdx as T);
        }
    }
}
