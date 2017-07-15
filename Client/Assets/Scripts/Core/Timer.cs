using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum TimerStatus { New, Update, Pause, Resume, Stop, Dispose }

public class Timer
{
    public delegate void UpdateTimerDelegate(float time);
    public delegate void CompleteTimerDelegate();
    public event UpdateTimerDelegate UpdateTimerCallback;
    public event CompleteTimerDelegate CompleteTimerCallback;
    public TimerStatus TimerStatus;
    public float UpdateInterval { get; private set; }
    public int IntervalCount { get; private set; }
    public float remainingTime { get; private set; }
    public float LastFramTime;
    #region TimerManager
    public static List<Timer> UpdateTimerList = new List<Timer>();

    public static Timer GetTimer(float TimeSpan)
    {
        return GetTimer(1, TimeSpan);
    }

    public static Timer GetTimer(float UpdateInterval, float TimeSpan)
    {

        Timer timer = new Timer();
        timer.remainingTime = TimeSpan;
        timer.UpdateInterval = UpdateInterval;
        timer.IntervalCount = Mathf.CeilToInt(timer.remainingTime / timer.UpdateInterval);
        return timer;
    }

    public static void DisposeAllTimer()
    {
        for (int i = 0; i < UpdateTimerList.Count; i++)
        {
            UpdateTimerList[i].DisposeTimer();
        }
    }
    #endregion

    /// <summary>
    /// 重置Timer, 如果在运行, 也会被停止
    /// </summary>
    public void ResetTimer(float updateInterval, float TimeSpan)
    {
        UpdateTimerList.Remove(this);
        remainingTime = TimeSpan;
        UpdateInterval = updateInterval;
        IntervalCount = Mathf.CeilToInt(remainingTime / UpdateInterval);
    }

    public void ResetTimer(float TimeSpan)
    {
        ResetTimer(1, TimeSpan);
    }

    public void StartTimer()
    {
        this.TimerStatus = TimerStatus.Update;
        UpdateTimerList.Add(this);
        LastFramTime = Time.realtimeSinceStartup;
    }


    public void PauseTimer()
    {
        this.TimerStatus = TimerStatus.Pause;
        UpdateTimerList.Remove(this);
    }


    public void ResumeTimer()
    {
        this.TimerStatus = TimerStatus.Update;
        UpdateTimerList.Add(this);
        LastFramTime = Time.realtimeSinceStartup;
    }


    public void DisposeTimer()
    {
        UpdateTimerList.Remove(this);
        this.Dispose();
    }


    public void Dispose()
    {

    }


    public void CompleteTimer()
    {
        this.StopTimer();
        this.DisposeTimer();
    }


    public void StopTimer()
    {
        UpdateTimerList.Remove(this);
    }


    float IntervalTime = 0;
    float deltaTime = 0;
    public void UpdateTimer(float deltaTime1)
    {
        if (remainingTime <= 0)
        {
            if (CompleteTimerCallback != null)
            {
                CompleteTimerCallback();
            }
            StopTimer();
            return;
        }
        deltaTime = Time.realtimeSinceStartup - LastFramTime;
        LastFramTime = Time.realtimeSinceStartup;
        if (IntervalTime >= UpdateInterval)
        {
            IntervalTime = 0;
            IntervalCount--;
            if (UpdateTimerCallback != null)
            {
                UpdateTimerCallback(remainingTime);
            }

        }
        IntervalTime += deltaTime;
        remainingTime -= deltaTime;
    }
}
