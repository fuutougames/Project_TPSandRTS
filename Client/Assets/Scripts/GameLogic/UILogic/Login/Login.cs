using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Login : WindowBase {
    private InputField m_ifAcc;
    private GameObject m_objBTNLogin;
    private GameObject m_objBTNExit;

    public override void Init()
    {
        base.Init();
        m_ifAcc = m_transInstanceRoot.Find("Acc").GetComponent<InputField>();
        m_objBTNLogin = m_transInstanceRoot.Find("BtnLogin").gameObject;
        m_objBTNExit = m_transInstanceRoot.Find("BtnExit").gameObject;
        UGUIEvtHandler.AddListener(m_objBTNLogin, UGUIEvtType.POINTER_CLICK, OnLoginClick);
        UGUIEvtHandler.AddListener(m_objBTNExit, UGUIEvtType.POINTER_CLICK, OnExit);
    }

    public override void StartUp(params object[] paramArr)
    {
        base.StartUp(paramArr);
    }

    private void OnLoginClick(PointerEventData evtDat)
    {
        Debug.Log(m_ifAcc.text);
    }

    private void OnExit(PointerEventData evtDat)
    {
        Application.Quit();
    }
}
