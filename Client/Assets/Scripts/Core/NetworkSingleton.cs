﻿//using UnityEngine;
//using System;

//public abstract class NetworkSingleton : NetworkBase { }

//public abstract class NetworkSingleton<T> : NetworkSingleton where T : NetworkSingleton
//{
//    private static bool Compare<T>(T x, T y) where T : class
//    {
//        return x == y;
//    }

//    #region Singleton

//    private static T _instance = default(T);

//    public static T Instance
//    {
//        get
//        {
//            if (!Compare<T>(default(T), _instance)) return _instance;

//            InitInstance(true);
//            return _instance;
//        }
//    }

//    #endregion

//    protected override void OnAwake()
//    {
//        base.OnAwake();
//        InitInstance(false);
//    }

//    private static void InitInstance(bool shouldInitManager)
//    {
//        Type thisType = typeof(T);

//        _instance = FindObjectOfType<T>();

//        if (Compare<T>(default(T), _instance))
//        {
//            _instance = new GameObject(thisType.Name).AddComponent<T>();
//        }

//        //Won't call InitManager from Awake
//        if (shouldInitManager)
//        {
//            (_instance as NetworkSingleton<T>).InitSigleton();
//        }
//    }

//    public virtual void InitSigleton() { }
//}

