using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface IWindow
{
    int GetModuleID();
    int GetWinInstanceID();
    bool IsUniqeWindow();
    GameObject GetRoot();
    void BaseInit(int moduleId, int instanceId, GameObject root);
    void Init();
    void StartUp(params object[] paramArr);
    void StartListener();
    void RemoveListener();
    void Clear();
}
