using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Config
{
    [System.Serializable]
    public class TargetDict : SerializableDictionaryBase<CommEnum.CONTRACT_TARGET, Vector3> { };
    [CreateAssetMenu(fileName = "ContractConf", menuName = "Config/ContractConf", order = 1)]
    public class ContractConf : ScriptableObjectBase, IConfig<int, int, int>
    {
        [Header("MainKey")]
        public int ID;
        public string Title;
        public string Description;
        public Vector2[] Rewards;
        public TargetDict Targets;
        public string ActionScene;

        public int GetMainKey()
        {
            //throw new System.NotImplementedException();
            return ID;
        }

        public int GetSubKey1()
        {
            //throw new System.NotImplementedException();
            return default(int);
        }

        public int GetSubKey2()
        {
            //throw new System.NotImplementedException();
            return default(int);
        }
    }
}
