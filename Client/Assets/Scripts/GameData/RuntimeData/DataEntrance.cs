using System.Collections;
using System.Collections.Generic;
using Common;
using Data.DataMgr;
using Data.DataIns;

public class DataEntrance : Singleton<DataEntrance>
{
    private List<DataMgrBase> _DataList;

    public DataEntrance()
    {
        _DataList = new List<DataMgrBase>();
    }

    public void AddData(DataMgrBase data)
    {
        _DataList.Add(data);
    }

    public void Save(string path)
    {
        for (int i = 0; i < _DataList.Count; ++i)
        {
            object data = _DataList[i].EncodeData();
            _DataList[i].Save(path, data);
        }
    }

    public void Read(string path)
    {
        for (int i = 0; i < _DataList.Count; ++i)
        {
            object data = _DataList[i].Load(path);
            _DataList[i].DecodeData(data);
        }
    }
}
