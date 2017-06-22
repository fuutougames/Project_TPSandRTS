using UnityEngine.Networking;

/// <summary>
/// The base class for custom game class of NetworkBehaviour
/// </summary>
public class NetworkBase : NetworkBehaviour
{
    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }
    protected virtual void OnUpdate() { }
    protected virtual void OnFixedUpdate() { }
    protected virtual void OnLateUpdate() { }

    void Awake()
    {
        OnAwake();
    }

    void Start()
    {
        OnStart();
    }

    void Update()
    {
        OnUpdate();
    }

    void FixedUpdate()
    {
        OnFixedUpdate();
    }

    void LateUpdate()
    {
        OnLateUpdate();
    }

}
