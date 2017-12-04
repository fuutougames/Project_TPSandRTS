using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Timer
{
    private float m_Interval = .0f;
    private float m_Time = .0f;

    /// <summary>
    /// UpdateAction take remain time as parameter
    /// </summary>
    public Action<float> UpdateAction = null;
    public Action CompleteAction = null;

    private float m_StartTime;
    private float m_LastUpdateTime;
    private float m_PauseTime = .0f;

    public Timer()
    {

    }

    public Timer(float time, float interval = .0001f)
    {
        m_Time = time;
        m_Interval = interval;
        m_PauseTime = .0f;
    }

    public void Reset(float time, float interval = .0001f)
    {
        m_Time = time;
        m_Interval = interval;
        m_PauseTime = .0f;
    }

    public void Update()
    {
        float realtimeFromStartup = TimeMgr.Instance.GetRealTimeFromeStartup();
        float timeDiff = realtimeFromStartup - m_LastUpdateTime;
        float remainTime = m_StartTime + m_PauseTime + m_Time - realtimeFromStartup;
        remainTime = remainTime >= .0f ? remainTime : .0f;
        if (timeDiff > m_Interval)
        {
            if (UpdateAction != null)
            {
                UpdateAction(remainTime);
            }
            m_LastUpdateTime = realtimeFromStartup;
        }

        if (remainTime <= .0f)
        {
            if (CompleteAction != null)
                CompleteAction.Invoke();

            TimerMgr.Instance.UnregisterTimer(this);
        }
    }

    public void Pause()
    {
        m_LastUpdateTime = TimeMgr.Instance.GetRealTimeFromeStartup();
        TimerMgr.Instance.UnregisterTimer(this);
    }

    public void Resume()
    {
        float realtimeFromStartup = TimeMgr.Instance.GetRealTimeFromeStartup();
        m_PauseTime += realtimeFromStartup - m_LastUpdateTime;
        TimerMgr.Instance.RegisterTimer(this);
    }

    public void Start()
    {
        m_StartTime = TimeMgr.Instance.GetRealTimeFromeStartup();
        m_LastUpdateTime = m_StartTime;
        m_PauseTime = .0f;
        TimerMgr.Instance.RegisterTimer(this);
    }

    public void Stop(bool callComplete = true)
    {
        TimerMgr.Instance.UnregisterTimer(this);
        if (callComplete)
            if (CompleteAction != null)
                CompleteAction.Invoke();
    }
}
