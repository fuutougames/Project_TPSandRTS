using UnityEngine;
using Common;

public class WindowBase : IWindow
{
    protected int m_iInstanceID;
    protected GameObject m_objInstanceRoot;
    protected RectTransform m_transInstanceRoot;
    protected Ticker m_ticker;
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
    /// Being called when window open, always call after startlistener
    /// </summary>
    /// <param name="paramArr"></param>
    public virtual void StartUp(params object[] paramArr)
    {

    }

    /// <summary>
    /// Being called when window open, always call after init
    /// </summary>
    public virtual void StartListener()
    {

    }

    public virtual void RemoveListener()
    {

    }

    public virtual void Clear()
    {

    }

    protected void Close()
    {
        Dispatcher.Dispatch(GameEvents.CommEvt.WINDOW_CLOSE_EVENT, m_iInstanceID);
    }
}
