using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// widget of window, should create by WindowBase.CreateWidget
/// normally it won't be destroy manually, 
/// but if u really need to destroy it manually, use WindowBase.DestroyWidget
/// </summary>
public class WindowWidgetBase
{
    private WindowBase m_WinRoot;
    private RectTransform m_Root;
    public RectTransform Root { get { return m_Root; } }

    public virtual void BaseInit(WindowBase winRoot, RectTransform root)
    {
        m_WinRoot = winRoot;
        m_Root = root;
    }

    public virtual void Init()
    {

    }

    public virtual void StartUp(params object[] paramArr)
    {

    }

    public virtual void RegisterListeners()
    {

    }

    public virtual void UnregisterListeners()
    {

    }

    public virtual void Clear()
    {

    }
}
