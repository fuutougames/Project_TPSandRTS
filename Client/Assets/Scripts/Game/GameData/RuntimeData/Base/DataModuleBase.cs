using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    public class DataModuleBase : IRecord
    {
        public DataModuleBase()
        {
            Debug.Log("RuntimeDataBase constructor called!!!");
            Date.Instance.AddData(this);
        }

        public virtual object EncodeData() { return null; }
        public int Save(string path, object data)
        {
            return 0;
        }

        public object Load(string path)
        {
            return null;
        }
        public virtual int DecodeData(object data) { return 0; }
    }
}
