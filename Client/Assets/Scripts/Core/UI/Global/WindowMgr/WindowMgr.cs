using System;
using System.Collections.Generic;
using Common;
using GameEvents;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private Queue<WinStartUpData> m_qOpenQueue = new Queue<WinStartUpData>();


    public WindowMgr()
    {
        Dispatcher.RegisterHandler(CommEvt.WINDOW_OPEN_EVENT, OnWindowOpen);
        Dispatcher.RegisterHandler(CommEvt.WINDOW_CLOSE_EVENT, OnWindowClose);
    }


    private Dictionary<int, int> m_dictCurrentInsIdTail = new Dictionary<int, int>();
    public int GenerateInstanceID(int moduleId)
    {
        int instanceIdTail;
        if (!m_dictCurrentInsIdTail.TryGetValue(moduleId, out instanceIdTail))
        {
            m_dictCurrentInsIdTail[moduleId] = 0;
            instanceIdTail = 0;
        }
        int instanceId = moduleId * 100 + (instanceIdTail % 100);
        for (int i = 0; i < 100 && m_hsUsedInsID.Contains(instanceId); ++i)
        {
            instanceIdTail += 1;
            instanceId = moduleId * 100 + (instanceIdTail % 100);
            if (i == 99)
            {
                if (m_hsUsedInsID.Contains(instanceId))
                {
                    throw new Exception(string.Format("Too Much Window Instance of Module-{0}", moduleId));
                }
            }
        }
        m_dictCurrentInsIdTail[moduleId] = instanceIdTail + 1;
        m_hsUsedInsID.Add(instanceId);
        return instanceId;
    }

    private void OnWindowOpen(params object [] paramArr)
    {
        int moduleId = (int)paramArr[0];
        WinStartUpData evt = new WinStartUpData();
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

    private void OnWindowOpen(WinStartUpData param)
    {
        if (!m_UISceneLoaded)
        {
            m_qOpenQueue.Enqueue(param);
            return;
        }

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
        // shows that this window is a unique window,
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
                tmpIns.GetRoot().transform.SetAsLastSibling();
                tmpIns.GetRoot().SetActive(true);
                tmpIns.RegisterListeners();
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

        // not in the cache, but it's not a unique window
        tmpIns = GetWinInstance(moduleId);
        //tmpIns.SetWindowInfo(moduleId);
        //if (!tmpIns.IsUniqeWindow())
        if (!WindowInfoMgr.GetWindowInfo(moduleId).UniqeWindow)
        {
            WinTemplate template;
            // resources is already loaded
            // create a new window immediately
            instanceId = GenerateInstanceID(moduleId);
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
                    tmpIns.RegisterListeners();
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
            instance.RegisterListeners();
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
                instance.UnregisterListeners();
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

    // driven by unity update event
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


    #region Initialize
    private GameObject m_objRootCanvas;
    private GameObject m_objEventSystem;
    private GameObject m_objWindowRoot;

    private RectTransform m_transRootCanvas;
    public RectTransform RootCanvasTrans { get { return m_transRootCanvas; } }
    private RectTransform m_transWindowRoot;
    public RectTransform WindowRootTrans { get { return m_transWindowRoot; } }

    private Canvas m_compRootCanvas;
    public Canvas RootCanvas { get { return m_compRootCanvas; } }
    private CanvasScaler m_compRootCanvasScaler;
    public CanvasScaler RootCanvasScaler { get { return m_compRootCanvasScaler; } }
    private GraphicRaycaster m_compRootGraphicRaycaster;
    public GraphicRaycaster RootGraphicRaycaster { get { return m_compRootGraphicRaycaster; } }

    private EventSystem m_compEventSystem;
    public EventSystem EventSystem { get { return m_compEventSystem; } }
    private StandaloneInputModule m_compSIModule;
    public StandaloneInputModule InputModule { get { return m_compSIModule; } }

    private Ticker m_compTicker;
    public Ticker UITicker { get { return m_compTicker; } }

    private bool m_UISceneLoaded = false;

    public void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
    }

    private void UISceneLoadedCallback(Scene scene, LoadSceneMode mode)
    {
        GameObject uiSceneRoot = GameObject.Find("UGUIRoot");
        GameObject.DontDestroyOnLoad(uiSceneRoot);
        Transform uiSceneRootTrans = uiSceneRoot.transform;
        m_objRootCanvas = uiSceneRootTrans.Find("RootCanvas").gameObject;
        m_objEventSystem = uiSceneRootTrans.Find("RootCanvas/EventSystem").gameObject;
        m_objWindowRoot = uiSceneRootTrans.Find("RootCanvas/WindowRoot").gameObject;

        m_transRootCanvas = m_objRootCanvas.GetComponent<RectTransform>();
        m_transWindowRoot = m_objWindowRoot.GetComponent<RectTransform>();


        m_compRootCanvas = m_objRootCanvas.GetComponent<Canvas>();
        m_compRootCanvasScaler = m_objRootCanvas.GetComponent<CanvasScaler>();
        m_compRootGraphicRaycaster = m_objRootCanvas.GetComponent<GraphicRaycaster>();

        m_compEventSystem = m_objEventSystem.GetComponent<EventSystem>();
        m_compSIModule = m_objEventSystem.GetComponent<StandaloneInputModule>();

        m_compTicker = m_objRootCanvas.GetComponent<Ticker>();
        m_compTicker.RegisterUpdateFunc(OnTryOpenBufferedWindow);

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "UIScene")
        {
            m_UISceneLoaded = true;
            UISceneLoadedCallback(scene, mode);
        }
    }
    #endregion
}

