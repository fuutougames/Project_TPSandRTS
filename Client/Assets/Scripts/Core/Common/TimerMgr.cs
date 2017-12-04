using System;
using System.Collections.Generic;

public class TimerMgr : MonoSingleton<TimerMgr> 
{
    private ObjPool<Timer> m_TimerPool;

    private HashSet<Timer> m_TickingTimer;
    private HashSet<Timer> m_AddToUpdateListBuffer;
    private HashSet<Timer> m_RemoveFromUpdateListBuffer;
    private HashSet<Timer> m_ReturnBuffer;

    private bool m_Ticking = false;

    public override void InitSigleton()
    {
        base.InitSigleton();
        DontDestroyOnLoad(this.gameObject);
        m_TimerPool = new ObjPool<Timer>();
        
        m_TickingTimer = new HashSet<Timer>();
        m_AddToUpdateListBuffer = new HashSet<Timer>();
        m_RemoveFromUpdateListBuffer = new HashSet<Timer>();
        m_ReturnBuffer = new HashSet<Timer>();
    }

    public Timer GetTimer()
    {
        return m_TimerPool.Pop();
    }

    public void ReturnTimer(ref Timer retTimer)
    {
        if (retTimer == null)
            return;

        Timer timer = retTimer;
        retTimer = null;
        if (m_TickingTimer.Contains(timer))
        {
            if (m_ReturnBuffer.Contains(timer))
                return;
            m_ReturnBuffer.Add(timer);
            return;
        }
        m_TimerPool.Push(timer);
    }

    public void RegisterTimer(Timer timer)
    {
        if (m_AddToUpdateListBuffer.Contains(timer) || m_TickingTimer.Contains(timer))
        {
            return;
        }

        if (m_Ticking)
        {
            m_AddToUpdateListBuffer.Add(timer);
            return;
        }

        m_TickingTimer.Add(timer);
    }

    public void UnregisterTimer(Timer timer)
    {
        if (m_Ticking)
        {
            m_RemoveFromUpdateListBuffer.Add(timer);
            return;
        }

        m_TickingTimer.Remove(timer);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        HashSet<Timer>.Enumerator iter;
        if (m_AddToUpdateListBuffer.Count > 0)
        {
            iter = m_AddToUpdateListBuffer.GetEnumerator();
            while (iter.MoveNext())
            {
                m_TickingTimer.Add(iter.Current);
            }
            m_AddToUpdateListBuffer.Clear();
        }

        m_Ticking = true;
        if (m_TickingTimer.Count > 0)
        {
            iter = m_TickingTimer.GetEnumerator();
            while (iter.MoveNext())
            {
                try
                {
                    iter.Current.Update();
                }
                catch (Exception e)
                {
#if UNITY_EDITOR
                    UnityEngine.Debug.LogError(string.Format("{0}\n{1}", e.Message, e.StackTrace));
#endif
                }
            }
        }
        m_Ticking = false;

        if (m_RemoveFromUpdateListBuffer.Count > 0)
        {
            iter = m_RemoveFromUpdateListBuffer.GetEnumerator();
            while (iter.MoveNext())
            {
                m_TickingTimer.Remove(iter.Current);
            }
            m_RemoveFromUpdateListBuffer.Clear();
        }

        if (m_ReturnBuffer.Count > 0)
        {
            iter = m_ReturnBuffer.GetEnumerator();
            while (iter.MoveNext())
            {
                m_TimerPool.Push(iter.Current);
            }
            m_ReturnBuffer.Clear();
        }
    }
}

