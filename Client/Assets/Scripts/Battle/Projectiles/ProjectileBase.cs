using Battle;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// ProjectileBase
/// Start position, direction, real range and state synchronize;
/// </summary>
public class ProjectileBase : NetworkBase
{
    #region Sync Var
    [SyncVar(hook = "OnProjectileTrigger")] protected bool _SyncIsTriggered = false;
    [SyncVar(hook = "OnProjectileStartPosSet")] protected Vector3 _SyncStartPos;
    [SyncVar(hook = "OnProjectileDirectionSet")] protected Vector3 _SyncDirection;
    [SyncVar] protected float _SyncRealRange;
    #endregion

    #region Inner Var
    protected Transform _CachedTransform;
    protected Transform CachedTransform
    {
        get
        {
            if (_CachedTransform == null)
                _CachedTransform = transform;

            return _CachedTransform;
        }
    }
    protected bool _IsFirstFrameIgnored = false;
    #endregion



    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (!_SyncIsTriggered || !_IsFirstFrameIgnored)
            return;

        UpdatePosition();
    }

    protected override void OnLateUpdate()
    {
        base.OnLateUpdate();
        if (_SyncIsTriggered && !_IsFirstFrameIgnored)
        {
            CachedTransform.position = _SyncStartPos;
            _IsFirstFrameIgnored = true;
        }
    }

    /// <summary>
    /// normal init
    /// </summary>
    public virtual void Init()
    {
        
    }

    /// <summary>
    /// server init
    /// </summary>
    [Server]
    public virtual void ServerInit(object param)
    {
        
    }

    /// <summary>
    /// Position Update Function
    /// </summary>
    protected virtual void UpdatePosition()
    {

    }

    [Client]
    protected void OnProjectileTrigger(bool isTrigger)
    {
        bool oldVal = _SyncIsTriggered;
        _SyncIsTriggered = isTrigger;
        if (!oldVal)
            _IsFirstFrameIgnored = false;
    }

    [Client]
    protected void OnProjectileStartPosSet(Vector3 startPos)
    {
        _SyncStartPos = startPos;
        CachedTransform.position = _SyncStartPos;
    }

    [Client]
    protected void OnProjectileDirectionSet(Vector3 direction)
    {
        _SyncDirection = direction;
        CachedTransform.forward = _SyncDirection;
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void BeforeProjectileTrigger()
    {
        
    }
    
    protected virtual void OnProjectileTrigger()
    {
        
    }
    
    protected virtual void AfterProjectileTrigger()
    {
        
    }

    /// <summary>
    /// Client will need this function too
    /// </summary>
    /// <param name="time"></param>
    /// <param name="characterCollider"></param>
    /// <returns>Hit Point</returns>
    public virtual bool IsCollideWithCharacter(float time, CapsuleCollider characterCollider, out Vector3 hitPoint)
    {
        hitPoint = Vector3.zero;
        return true;
    }

    /// <summary>
    /// Default Projectile Trigger Function;
    /// </summary>
    [Server]
    public void TriggerProjectile()
    {
        _SyncIsTriggered = true;
        _IsFirstFrameIgnored = false;
        OnProjectileTrigger();
    }


    [Server]
    public virtual float CalculateDamage(Vector3 hitPoint, BattleCharacterData character)
    {
        return 0;
    }

    public void RegisterProjectile()
    {
        
    }

    public void UnRegisterProjectile()
    {
        
    }
}