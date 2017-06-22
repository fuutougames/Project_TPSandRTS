using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.DataMgr
{
    public class DataMgrBase
    {
        public DataMgrBase()
        {
            Debug.Log("RuntimeDataBase constructor called!!!");
            DataEntrance.Instance.AddData(this);
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
