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
    [SyncVar(hook = "OnProjectileTerminated")] protected bool _Terminated;
    //[SyncVar] protected float _SyncRealRange;
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
    protected float _RealRange;
    #endregion

    #region Client Server Related
    /// <summary>
    /// Default Projectile Trigger Function;
    /// </summary>
    [ServerCallback]
    public void TriggerProjectile(Vector3 startPos, Vector3 direction)
    {
        _SyncStartPos = startPos;
        _SyncDirection = direction;
        // must call after pos and dir set;
        BeforeProjectileTrigger();
        _SyncIsTriggered = true;
        _IsFirstFrameIgnored = false;
        _Terminated = false;
        OnProjectileTrigger();
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

    [Client]
    protected void OnProjectileTerminated(bool terminated)
    {
        _Terminated = terminated;
        if (_Terminated)
        {
            // client terminate this projectile
        }
    }
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
    public virtual void Init(ProjectileBattleData data)
    {
        
    }

    /// <summary>
    /// Position Update Function
    /// </summary>
    protected virtual void UpdatePosition()
    {

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
    
    public virtual float CalculateDamage(Vector3 hitPoint, CharacterBattleData character)
    {
        return 0;
    }

    public void RegisterProjectile()
    {
        BattleMgr.Instance.BData.RegisterProjectile(this);
    }

    public void UnRegisterProjectile()
    {
        BattleMgr.Instance.BData.UnRegisterProjectile(this.GetInstanceID());
    }
}