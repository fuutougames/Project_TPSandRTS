using UnityEngine;
using System.Collections;
using GameEvents;

public class Entrance : MonoBehaviour 
{

    void Start()
    {
        WindowMgr.Instance.Init();

        Dispatcher.Dispatch(CommEvt.WINDOW_OPEN_EVENT, UIModule.LOGIN, 1, 2, 3, 4);
    }
}
