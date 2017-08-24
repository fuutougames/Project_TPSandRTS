using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//using Rendering;
using Common;
using UnityEngine.SceneManagement;

public partial class WindowMgr
{
    private GameObject m_objRootCanvas;
    private GameObject m_objUICam;
    private GameObject m_objEventSystem;
    private GameObject m_objWindowRoot;

    private RectTransform m_transRootCanvas;
    public RectTransform RootCanvasTrans { get { return m_transRootCanvas; } }
    private RectTransform m_transWindowRoot;
    public RectTransform WindowRootTrans { get { return m_transWindowRoot; } }

    private Camera m_camUICam;
    public Camera UICam { get { return m_camUICam; } }

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
        m_objUICam = uiSceneRootTrans.Find("UICamera").gameObject;
        m_objEventSystem = uiSceneRootTrans.Find("RootCanvas/EventSystem").gameObject;
        m_objWindowRoot = uiSceneRootTrans.Find("RootCanvas/WindowRoot").gameObject;

        m_transRootCanvas = m_objRootCanvas.GetComponent<RectTransform>();
        m_transWindowRoot = m_objWindowRoot.GetComponent<RectTransform>();

        m_camUICam = m_objUICam.GetComponent<Camera>();

        m_compRootCanvas = m_objRootCanvas.GetComponent<Canvas>();
        m_compRootCanvasScaler = m_objRootCanvas.GetComponent<CanvasScaler>();
        m_compRootGraphicRaycaster = m_objRootCanvas.GetComponent<GraphicRaycaster>();

        m_compEventSystem = m_objEventSystem.GetComponent<EventSystem>();
        m_compSIModule = m_objEventSystem.GetComponent<StandaloneInputModule>();

        m_compTicker = m_objRootCanvas.GetComponent<Ticker>();

        //RenderingUnit unit = new RenderingUnit("UINode", m_camUICam);
        //RenderingMgr.Instance.AddCrucialUnitAtLast("UINode", unit);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "UIScene")
        {
            UISceneLoadedCallback(scene, mode);
        }
    }
}
