using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourceManager : MonoSingleton<ResourceManager>
{
    public override void InitSigleton()
    {
        base.InitSigleton();
    }

    public C LoadResource<C>(string path) where C : UnityEngine.Object
    {
        return Resources.Load<C>(path);
    }
}