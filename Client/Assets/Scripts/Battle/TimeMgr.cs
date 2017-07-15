using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeMgr : MonoSingleton<TimeMgr> {

    public float GetCurrentTime ()
    {
        return Time.realtimeSinceStartup;
    }

    public float GetDeltaTime ()
    {
        return Time.deltaTime;
    }
}
