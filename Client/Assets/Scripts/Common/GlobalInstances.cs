using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class GlobalInstances : Singleton<GlobalInstances>
{
    public RND_MT19937_32 RndIns { get; private set; }

    public GlobalInstances()
    {
        RndIns = new RND_MT19937_32();
        RndIns.srnd((uint)DateTime.Now.Second);
    }
}
