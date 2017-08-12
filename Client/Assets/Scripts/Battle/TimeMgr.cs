using UnityEngine;

public class TimeMgr : MonoSingleton<TimeMgr>
{
    private double accumDeltaTime = .0d;

    private void Awake()
    {
        accumDeltaTime = .0d;


        DontDestroyOnLoad(gameObject);
    }

    // Get Current accumulate time of Time.deltaTime from game start up
    public float GetCurrentTime ()
    {
        return (float)accumDeltaTime;
    }

    public float GetDeltaTime ()
    {
        return Time.deltaTime;
    }

    public float GetRealTimeFromeStartup()
    {
        return Time.realtimeSinceStartup;
    }


    protected override void OnFixedUpdate()
    {
        //base.OnFixedUpdate();
        accumDeltaTime += Time.deltaTime;
    }
}
