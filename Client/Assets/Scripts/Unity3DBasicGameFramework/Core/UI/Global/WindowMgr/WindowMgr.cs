﻿using System.Collections.Generic;
using Common;
using GameEvents;
using UnityEngine;

public partial class WindowMgr : Singleton<WindowMgr>
{
    public class WinTemplate
    {
        public int RefCnt;
        public int ModuleID;
        public UnityEngine.Object Template;
        public static bool operator ==(WinTemplate x, WinTemplate y)
        {
            return x.ModuleID == y.ModuleID ? true : false;
        }
        public static bool operator !=(WinTemplate x, WinTemplate y)
        {
            return x.ModuleID == y.ModuleID ? false : true;
        }
        public override bool Equals(object obj)
        {
            WinTemplate target = (WinTemplate)obj;
            return target.ModuleID == this.ModuleID ? true : false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    private static readonly int NONUNIQE_WINDOW_TEMPLATE_CACHE_SIZE = 16;
    private static readonly int WINDOW_INSTANCE_CAHCE_SIZE = 16;

    private HashSet<int> m_hsUsedInsID = new HashSet<int>();
    // <instanceId, IWindowBase>
    private Dictionary<int, IWindow> m_dictInstanceMapper = new Dictionary<int, IWindow>();
    private LinkedList<IWindow> m_llInstances = new LinkedList<IWindow>();
    // <moduleId, WinTemplate>
    private Dictionary<int, WinTemplate> m_dictTemplateMapper = new Dictionary<int, WinTemplate>();
    private LinkedList<WinTemplate> m_llWinTemplate = new LinkedList<WinTemplate>();

    private bool m_bIsOpeningAWindow = false;
    private Queue<WindowStartUpEvent> m_qOpenQueue = new Queue<WindowStartUpEvent>();


    public WindowMgr()
    {
        Dispatcher.RegisterHandler(GameEvents.CommEvt.WINDOW_OPEN_EVENT, OnWindowOpen);
        //Dispatcher.RegisterHandler(WindowStartUpEvent.EVT_NAME, OnWindowOpen);
        Dispatcher.RegisterHandler(GameEvents.CommEvt.WINDOW_CLOSE_EVENT, OnWindowClose);
    }

    private void OnWindowOpen(params object [] paramArr)
    {
        int moduleId = (int)paramArr[0];
        WindowStartUpEvent evt = new WindowStartUpEvent();
        evt.ModuleID = moduleId;
        evt.Params = null;
        if (paramArr.Length >= 2)
        {
            List<object> subParamArr = new List<object>();
            for (int i = 1; i < paramArr.Length; ++i)
            {
                subParamArr.Add(paramArr[i]);
            }

            evt.Params = subParamArr.ToArray();
        }

        OnWindowOpen(evt);
    }

    //private void OnWindowOpen(int moduleId, params object[] paramArr)
    private void OnWindowOpen(WindowStartUpEvent param)
    {
        if (m_bIsOpeningAWindow)
        {
            // another window is loading, put this one into open queue and return;
            m_qOpenQueue.Enqueue(param);
            return;
        }

        m_bIsOpeningAWindow = true;
        int moduleId = param.ModuleID;
        int instanceId = moduleId;
        IWindow tmpIns;
        // if key moduleId exist in m_dictInstanceMapper,
        // shows that this window is a uniqe window,
        // and it's in the cache;
        if (m_dictInstanceMapper.TryGetValue(instanceId, out tmpIns))
        {
            LinkedListNode<IWindow> node = m_llInstances.Find(tmpIns);
            m_llInstances.Remove(node);
            m_llInstances.AddFirst(node);

#if !_DEBUG
            try
            {
#endif
                tmpIns.GetRoot().SetActive(true);
                tmpIns.GetRoot().transform.SetAsLastSibling();
                tmpIns.StartListener();
                tmpIns.StartUp(param.Params);
#if !_DEBUG
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e);
            }
#endif
            m_bIsOpeningAWindow = false;
            return;
        }

        // not in the cache, but it's not a uniqe window
        tmpIns = GetWinInstance(moduleId);
        //tmpIns.SetWindowInfo(moduleId);
        //if (!tmpIns.IsUniqeWindow())
        if (!WindowInfoMgr.GetWindowInfo(moduleId).UniqeWindow)
        {
            WinTemplate template;
            // resources is already loaded
            // create a new window immediately
            instanceId = GetInstanceID(moduleId);
            if (m_dictTemplateMapper.TryGetValue(moduleId, out template))
            {
                GameObject root = GameObject.Instantiate(template.Template) as GameObject;
                root.transform.SetParent(m_transWindowRoot, false);
                root.transform.SetAsLastSibling();
                root.SetActive(true);
                template.RefCnt += 1;
                //LinkedListNode<WinTemplate> node = m_llWinTemplate.Find(template);
                //m_llWinTemplate.Remove(node);
                //m_llWinTemplate.AddFirst(template);
                m_dictInstanceMapper.Add(instanceId, tmpIns);
                //m_dictTemplateMapper[moduleId] = template;
#if !_DEBUG
                try
                {
#endif
                    tmpIns.BaseInit(moduleId, instanceId, root);
                    tmpIns.Init();
                    tmpIns.StartListener();
                    tmpIns.StartUp(param.Params);
#if !_DEBUG
                }
                catch (Exception e)
                {
                    ExceptionHandler.LogException(e);
                }
#endif
                m_bIsOpeningAWindow = false;
                return;
            }
        }

        IntermediateParams.WinLoadedParam winParam = new IntermediateParams.WinLoadedParam();
        winParam.ModuleID = moduleId;
        winParam.InstanceID = instanceId;
        winParam.StartParam = param.Params;
        winParam.Instance = tmpIns;
        string path = Path.GetUIPath(WindowInfoMgr.GetWindowInfo(moduleId).ResName);
        ResLoader.Load(path, OnWindowResLoaded, winParam);
    }

    private void OnWindowResLoaded(UnityEngine.Object res, object param)
    {
        IntermediateParams.WinLoadedParam interParam = param as IntermediateParams.WinLoadedParam;
        IWindow instance = interParam.Instance;
        if (instance.IsUniqeWindow())
        {
            // clean job
            if (m_llInstances.Count >= WINDOW_INSTANCE_CAHCE_SIZE)
            {
                LinkedListNode<IWindow> iter = m_llInstances.Last;
                for (; iter != null; iter = iter.Previous)
                {
                    if (!iter.Value.GetRoot().activeSelf)
                    {
                        m_llInstances.Remove(iter);
                        m_dictInstanceMapper.Remove(iter.Value.GetWinInstanceID());
                        GameObject.Destroy(iter.Value.GetRoot());
                        break;
                    }
                }
            }
        }
        else
        {
            // clean job
            if (m_llWinTemplate.Count >= NONUNIQE_WINDOW_TEMPLATE_CACHE_SIZE)
            {
                LinkedListNode<WinTemplate> iter = m_llWinTemplate.Last;
                for (; iter != null; iter = iter.Previous)
                {
                    if (iter.Value.RefCnt <= 0)
                    {
                        m_llWinTemplate.Remove(iter);
                        m_dictTemplateMapper.Remove(iter.Value.ModuleID);
                        break;
                    }
                }
            }

            WinTemplate template = new WinTemplate()
            {
                ModuleID = interParam.ModuleID,
                RefCnt = 1,
                Template = res
            };
            m_llWinTemplate.AddFirst(template);
            m_dictTemplateMapper.Add(template.ModuleID, template);
        }

        //create new window
        GameObject root = GameObject.Instantiate(res) as GameObject;
        root.transform.SetParent(m_transWindowRoot, false);
        root.transform.SetAsLastSibling();
        root.SetActive(true);
        m_llInstances.AddFirst(instance);
        m_dictInstanceMapper.Add(interParam.InstanceID, instance);
#if !_DEBUG
        try
        {
#endif
            instance.BaseInit(interParam.ModuleID, interParam.InstanceID, root);
            instance.Init();
            instance.StartListener();
            instance.StartUp(interParam.StartParam);
#if !_DEBUG
        }
        catch (Exception e)
        {
            ExceptionHandler.LogException(e);
        }
#endif
        m_bIsOpeningAWindow = false;
    }

    private void OnWindowClose(params object [] paramArr)
    {
        int instanceId = (int)paramArr[0];
        IWindow instance;
        if (m_dictInstanceMapper.TryGetValue(instanceId, out instance))
        {
#if !_DEBUG
            try
            {
#endif
                instance.RemoveListener();
                instance.Clear();
#if !_DEBUG
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e);
            }
#endif
            if (instance.IsUniqeWindow())
            {
                instance.GetRoot().SetActive(false);
            }
            else
            {
                m_llInstances.Remove(instance);
                m_dictInstanceMapper.Remove(instanceId);
                WinTemplate template;
                if (m_dictTemplateMapper.TryGetValue(instance.GetModuleID(), out template))
                {
                    template.RefCnt -= 1;
                    //m_dictTemplateMapper[instance.GetModuleID()] = template;
                }
                GameObject.Destroy(instance.GetRoot());
                m_hsUsedInsID.Remove(instance.GetWinInstanceID());
                instance = null;
            }
        }
    }

    private void OnTryOpenBufferedWindow(float dt)
    {
        // DO NOT PUT THIS CODEBLOCK INTO OnWindowLoaded or OnWindowOpen
        // OR IT MIGHT CAUSE UNEXPECTED PROBLEM BECAUSE OF POTENTIAL NESTING LOOP
        if (m_qOpenQueue.Count > 0)
        {
            for (int i = 0; i < m_qOpenQueue.Count; ++i)
            {
                if (m_bIsOpeningAWindow)
                    break;
                OnWindowOpen(m_qOpenQueue.Dequeue());
            }
        }
    }
}

