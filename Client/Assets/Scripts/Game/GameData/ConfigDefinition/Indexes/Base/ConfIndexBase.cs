using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Config
{
    /// <summary>
    /// Acts like a singleton, but not a real singleton
    /// </summary>
    /// <typeparam name="T">Type of ConfIndex</typeparam>
    public class ConfIndexBase<T> : ScriptableObjectBase where T: ScriptableObjectBase
    {
        public static T GetInstance()
        {
            T t = ConfMgr.Instance.GetConfIndex<T>();
            return t;
        }

        public static T Instance
        {
            get
            {
                return GetInstance();
            }
        }
    }
}
