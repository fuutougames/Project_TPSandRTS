using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

/// <summary>
/// widget of window, should create by WindowBase.CreateWidget
/// normally it won't be destroy manually, 
/// but if u really need to destroy it manually, use WindowBase.DestroyWidget
/// </summary>
public class WindowWidgetBase
{
    protected WindowBase m_winRoot;
    protected RectTransform m_root;
    public RectTransform Root { get { return m_root; } }
    //protected Ticker m_ticker;
    //public Ticker RootTicker { get { return m_ticker; } }

    private Ticker m_ticker = null;

    public Ticker RootTicker
    {
        get
        {
            if (m_ticker == null)
                m_ticker = m_root.gameObject.AddComponent<Ticker>();
            return m_ticker;
        }
    }
    

    public virtual void BaseInit(WindowBase winRoot, RectTransform root)
    {
        m_winRoot = winRoot;
        m_root = root;
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

    public virtual void Update(float dt)
    {

    }

    public virtual void FixedUpdate(float dt)
    {

    }

    public virtual void LateUpdate(float dt)
    {

    }

}
