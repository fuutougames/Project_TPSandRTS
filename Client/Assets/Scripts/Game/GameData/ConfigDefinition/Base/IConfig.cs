using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Config
{
    public interface IConfig<K1, K2, K3>
    {
        K1 GetMainKey();
        K2 GetSubKey1();
        K3 GetSubKey2();
    }
}
