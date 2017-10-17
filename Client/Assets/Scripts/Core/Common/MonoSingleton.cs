using UnityEngine;
using System;

public abstract class MonoSingleton : MonoBase { }

public abstract class MonoSingleton<T> : MonoSingleton where T : MonoSingleton
{
    private static bool Compare<T1>(T1 x, T1 y) where T1 : class
    {
        return x == y;
    }

    #region Singleton

    private static T _instance = default(T);

    public static T Instance
    {
        get
        {
            if (!Compare<T>(default(T), _instance)) return _instance;

            InitInstance(true);
            return _instance;
        }
    }

    #endregion

    protected override void OnAwake()
    {
        base.OnAwake();
        InitInstance(false);
    }

    public static void InitInstance(bool shouldInitManager)
    {
        Type thisType = typeof(T);

        _instance = FindObjectOfType<T>();

        if (Compare<T>(default(T), _instance))
        {
            _instance = new GameObject(thisType.Name).AddComponent<T>();
        }

        //Won't call InitManager from Awake
        if (shouldInitManager)
        {
            (_instance as MonoSingleton<T>).InitSigleton();
        }
    }

    public virtual void InitSigleton() { }
}

