using System.Collections;
using System.Collections.Generic;
using Common;
using GameData;

public class Date : Singleton<Date>
{
    #region Basic Functions
    private List<DataModuleBase> _DataList;

    public Date()
    {
        _DataList = new List<DataModuleBase>();
    }

    public void AddData(DataModuleBase data)
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
    #endregion


    #region Data Modules
    public ContractData CommissionData = new ContractData();
    #endregion
}
