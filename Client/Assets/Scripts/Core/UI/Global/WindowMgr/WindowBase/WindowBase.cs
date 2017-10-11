using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// base class of UI window in game
/// </summary>
public class WindowBase : IWindow
{
    protected int m_iInstanceID;
    public int InstanceID { get { return m_iInstanceID; } }
    protected GameObject m_objInstanceRoot;
    protected RectTransform m_transInstanceRoot;
    public RectTransform RootTrans { get { return m_transInstanceRoot; } }
    protected Ticker m_ticker;
    public Ticker RootTicker { get { return m_ticker; } }
    protected WindowInfo m_wiInfo;

    protected WindowBase()
    {

    }

    /// <summary>
    /// Get window module id
    /// </summary>
    /// <returns></returns>
    public int GetModuleID()
    {
        return m_wiInfo.ModuleID;
    }

    /// <summary>
    /// Get window instance id
    /// </summary>
    /// <returns></returns>
    public int GetWinInstanceID()
    {
        return m_iInstanceID;
    }

    public virtual bool IsUniqeWindow()
    {
        return m_wiInfo.UniqeWindow;
    }

    public GameObject GetRoot()
    {
        return m_objInstanceRoot;
    }

    public void BaseInit(int moduleId, int instanceId, GameObject root)
    {
        m_wiInfo = WindowInfoMgr.GetWindowInfo(moduleId);
        m_iInstanceID = instanceId;
        m_objInstanceRoot = root;
        m_transInstanceRoot = root.GetComponent<RectTransform>();
        m_ticker = m_objInstanceRoot.AddComponent<Ticker>();
    }

    /// <summary>
    /// Being called when window prefab is loaded
    /// </summary>
    public virtual void Init()
    {

    }

    /// <summary>
    /// Being called when window open, always call after StartListener
    /// </summary>
    /// <param name="paramArr"></param>
    public virtual void StartUp(params object[] paramArr)
    {
        HashSet<WindowWidgetBase>.Enumerator iter = m_Widgets.GetEnumerator();
        while (iter.MoveNext())
        {
            iter.Current.StartUp(paramArr);
        }
    }

    /// <summary>
    /// Being called when window open, always call after Init
    /// </summary>
    public virtual void RegisterListeners()
    {
        HashSet<WindowWidgetBase>.Enumerator iter = m_Widgets.GetEnumerator();
        while (iter.MoveNext())
        {
            iter.Current.RegisterListeners();
        }
    }

    public virtual void UnregisterListeners()
    {
        HashSet<WindowWidgetBase>.Enumerator iter = m_Widgets.GetEnumerator();
        while (iter.MoveNext())
        {
            iter.Current.UnregisterListeners();
        }
    }

    public virtual void Clear()
    {
        HashSet<WindowWidgetBase>.Enumerator iter = m_Widgets.GetEnumerator();
        while (iter.MoveNext())
        {
            iter.Current.Clear();
        }
    }

    protected void Close()
    {
        Dispatcher.Dispatch(GameEvents.CommEvt.WINDOW_CLOSE_EVENT, m_iInstanceID);
    }


    #region widget related

    private HashSet<WindowWidgetBase> m_Widgets = new HashSet<WindowWidgetBase>(); 

    public T CreateWidget<T> (RectTransform widgetRoot) where T : WindowWidgetBase, new ()
    {
        T t = new T();
        t.BaseInit(this, widgetRoot);
        t.Init();
        m_Widgets.Add(t);
        return t;
    }

    public void DestroyWidget<T> (T widget) where T : WindowWidgetBase, new()
    {
        widget.UnregisterListeners();
        widget.Clear();
        m_Widgets.Remove(widget);
        GameObject.DestroyImmediate(widget.Root.gameObject);
    }
    #endregion
}
