using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Battle;

public class ProjectileDmgLineNode
{
    public float TimeNode;
    public float RemainDamage;
    public ObstacleData Obstacle;
    public Vector3 TriggerPoint;
    public bool Triggred;
    public byte InOut; // 0 for in, 1 for out;
}

public class ProjectileDmgLine
{
    private Vector3 _StartPos;
    public Vector3 StartPos { get { return _StartPos; } set { _StartPos = value; } }
    private Vector3 _EndPos;
    public Vector3 EndPos { get { return _EndPos; } set { _EndPos = value; } }
    private float _RealRange;
    public float RealRange { get { return _RealRange; } set { _RealRange = value; } }
    private float _StartTime;
    private float _EndTime;
    private float _TimeClipLen;
    private List<ProjectileDmgLineNode> _Nodes;
    public List<ProjectileDmgLineNode> Nodes { get { return _Nodes; } } 
    private int _NodeIdx;
    public int NodeCount { get { return _NodeIdx; } }

    public ProjectileDmgLine()
    {
        _Nodes = new List<ProjectileDmgLineNode>();
        _NodeIdx = 0;
    }

    public void SetStartAndEndTime(float start, float end)
    {
        _StartTime = start;
        _EndTime = end;
        _TimeClipLen = end - start;
    }

    public void AddNode(float timeNode, float remainDamage, ObstacleData obstacle, Vector3 triggerPoint, byte inout)
    {
        if (_NodeIdx >= _Nodes.Count)
        {
            ProjectileDmgLineNode node = new ProjectileDmgLineNode()
            {
                TimeNode = timeNode,
                RemainDamage = remainDamage,
                Obstacle = obstacle,
                TriggerPoint = triggerPoint,
                Triggred = false,
                InOut = inout
            };
            _Nodes.Add(node);
        }
        else
        {
            ProjectileDmgLineNode node = _Nodes[_NodeIdx];
            node.TimeNode = timeNode;
            node.RemainDamage = remainDamage;
            node.Obstacle = obstacle;
            node.TriggerPoint = triggerPoint;
            node.Triggred = false;
            node.InOut = inout;
        }
        ++ _NodeIdx;
    }

    public void ClearNode()
    {
        _NodeIdx = 0;
    }

    public int GetPassedIdxByTime(float time)
    {
        for (int i = 0; i < _NodeIdx - 1; ++i)
        {
            if (time > _Nodes[i].TimeNode && time < _Nodes[i + 1].TimeNode)
                return i;
        }
        return _NodeIdx - 1;
    }

    /// <summary>
    /// Get Projectile Current Position By Time
    /// </summary>
    /// <param name="time">input TimeMgr.GetCurrentTime() here</param>
    /// <returns></returns>
    public Vector3 GetPositionByTime(float time)
    {
        float val = (time - _StartTime)/_TimeClipLen;
        return Vector3.Lerp(_StartPos, _EndPos, val);
    }

    /// <summary>
    /// Get Projectile Remain Damage By Time
    /// </summary>
    /// <param name="time">input TimeMgr.GetCurrentTime() here</param>
    /// <returns></returns>
    public float GetRemainDmgByTime(float time)
    {
        return _Nodes[GetPassedIdxByTime(time)].RemainDamage;
    }

    /// <summary>
    /// call to force terminate a DmgLine
    /// </summary>
    public void Terminate()
    {
        //TODO: Finish Terminate function
    }
}

public class LineProjectile : ProjectileBase
{
    // Client know all these data;
    private float _BaseDamage;
    private float _Velocity;
    private float _MaxRange;
    private float _Penetration;
    private ProjectileDmgLine _DmgLine;
    public Action<ProjectileDmgLineNode> OnCollideAction;

    ///// <summary>
    ///// Init SERVER ONLY infos, which is those sync var;
    ///// </summary>
    ///// <param name="param"></param>
    //[Server]
    //public override void ServerInit(object param)
    //{
    //    base.ServerInit(param);
    //}

    /// <summary>
    /// Init CLIENT & SERVER infos, like _BaseDamage, _Velocity, etc ...
    /// </summary>
    public override void Init(ProjectileBattleData data)
    {
        base.Init(data);
        _DmgLine = new ProjectileDmgLine();
        _BaseDamage = data.BaseDamage;
        _Velocity = data.Velocity;
        _MaxRange = data.MaxRange;
        _Penetration = data.Penetration;
    }

    /// <summary>
    /// projectile position update function
    /// both server and client will use it
    /// </summary>
    protected override void UpdatePosition()
    {
        if (!_SyncIsTriggered)
            return;

        CachedTransform.position += _SyncDirection*_Velocity*(float)TimeMgr.Instance.GetCurrentTime();
        if (Vector3.Distance(CachedTransform.position, _SyncStartPos) > _RealRange)
        {
            // TODO:dispose or recycle projectile here;
            DisposeProjectile();
        }
    }

    protected override void OnFixedUpdate()
    {
        if (!_SyncIsTriggered)
            return;

        base.OnFixedUpdate();
        float now = Time.realtimeSinceStartup;
        int curIdx = _DmgLine.GetPassedIdxByTime(now);
        for (int i = 0; i <= curIdx; ++i)
        {
            if (!_DmgLine.Nodes[i].Triggred)
            {
                if (_DmgLine.Nodes[i].Obstacle != null)
                {
                    OnCollide(_DmgLine.Nodes[i]);
                    if (OnCollideAction != null)
                        OnCollideAction.Invoke(_DmgLine.Nodes[i]);
                }
                _DmgLine.Nodes[i].Triggred = true;
            }
        }
    }

    /// <summary>
    /// both server and client will run this function 
    /// when the collide occur, this function will call with collide information
    /// </summary>
    protected virtual void OnCollide(ProjectileDmgLineNode node)
    {
#if UNITY_EDITOR
        Debug.Log("<color=cyan>Collide: "+node.Obstacle.transform.GetInstanceID()+"</color>");
#endif
    }

    /// <summary>
    /// Calculate Projectile's Damage Line before it's triggered;
    /// Client should have DmgLine data too, for showing hit effect by itself;
    /// </summary>
    protected override void BeforeProjectileTrigger()
    {
        base.BeforeProjectileTrigger();
        _DmgLine.ClearNode();
        Ray ray = new Ray(_SyncStartPos, _SyncDirection);
        RaycastHit[] hits = Physics.RaycastAll(ray, _MaxRange, ~GameLayer.ObstacleCollider);
        float dmgRemain = _BaseDamage;
        float projectileStartTime = Time.realtimeSinceStartup;
        _DmgLine.AddNode(projectileStartTime, dmgRemain, null, _SyncStartPos, 0);
        _DmgLine.StartPos = _SyncStartPos;
        bool projectileBlocked = false;
        for (int i = 0; i < hits.Length; ++i)
        {
            RaycastHit hit = hits[i];
            int instanceId = hit.transform.GetInstanceID();
            //TODO:Setup Obstacle Data in BattleMgr
            ObstacleData obstacle = BattleMgr.Instance.SceneData.GetObstacleDataByInstanceID(instanceId);
            // if not a stored obstacle, just break for now;
            if (!obstacle)
                break;

            float timeNode = projectileStartTime + hit.distance / _Velocity;
            // obstacle not penetrable, projectile stop here;
            if (!obstacle.Penetrable)
            {
                _DmgLine.SetStartAndEndTime(projectileStartTime, projectileStartTime + hit.distance/_Velocity);
                _RealRange = hit.distance;
                _DmgLine.EndPos = _SyncStartPos + _SyncDirection * hit.distance;
                _DmgLine.RealRange = _RealRange;
                _DmgLine.SetStartAndEndTime(projectileStartTime, timeNode);
                _DmgLine.AddNode(timeNode, 0, obstacle, _DmgLine.EndPos, 0);
                projectileBlocked = true;
                break;
            }

            // obstacle penetrable
            // calculate projectile's time-damage line node and store it;
            float penLen = float.MaxValue;
            Vector3 targetPoint = Vector3.zero;
            for (int j = 0; j < obstacle.CollideSurfaces.Length; ++j)
            {
                Plane plane = obstacle.CollideSurfaces[j];

                //filter surfaces which will never intersect with current ray.
                float dotResult = Vector3.Dot(plane.normal, ray.direction);
                if (Vector3.Dot(plane.normal, ray.direction) < .0f)
                    continue;

                float tmp;
                if (plane.Raycast(ray, out tmp))
                {
                    Vector3 point = ray.GetPoint(tmp);
                    float curLen = Vector3.Distance(point, hit.point);
                    if (curLen < penLen)
                    {
                        penLen = curLen;
                        targetPoint = point;
                    }
                }
                //else
                //{
                //    // error tororence;
                //    penLen = 0;
                //    targetPoint = hit.point;
                //}
            }
            if (Mathf.Abs(penLen - float.MaxValue) <= Mathf.Epsilon)
            {
                penLen = .0f;
                targetPoint = hit.point;
            }

            float dmgLost = CalcDmgLost(obstacle, penLen);
            dmgRemain -= dmgLost;
            // projectile lost all damage, stop here;
            if (dmgRemain <= 0)
            {
                // projectile stop here
                _RealRange = hit.distance;
                _DmgLine.EndPos = _SyncStartPos + _SyncDirection * hit.distance;
                _DmgLine.RealRange = _RealRange;
                _DmgLine.SetStartAndEndTime(projectileStartTime, timeNode);
                _DmgLine.AddNode(timeNode, 0, obstacle, hit.point, 0);
                projectileBlocked = true;
                break;
            }
            _DmgLine.AddNode(timeNode, dmgRemain, obstacle, hit.point, 0);
            _DmgLine.AddNode(timeNode, dmgRemain, obstacle, targetPoint, 1);
        }

        // if projectile is not blocked, it should stop at it's max range;
        if (!projectileBlocked)
        {
            _RealRange = _MaxRange;
            _DmgLine.EndPos = _SyncStartPos + _SyncDirection * _MaxRange;
            _DmgLine.RealRange = _MaxRange;
            float timeNode = projectileStartTime + _MaxRange/_Velocity;
            _DmgLine.SetStartAndEndTime(projectileStartTime, timeNode);
            _DmgLine.AddNode(timeNode, dmgRemain, null, _DmgLine.EndPos, 0);
        }
    }
    
    /// <summary>
    /// Calculate the damage lost after bullet go through an obstacle;
    /// both server and client will need this function
    /// </summary>
    /// <param name="obstacle"></param>
    /// <param name="penLen"></param>
    /// <returns></returns>
    protected float CalcDmgLost(ObstacleData obstacle, float penLen)
    {
        //TODO: Calculate penetration damage lost;
        return 0;
    }

    public override bool IsCollideWithCharacter(float time, CapsuleCollider characterCollider, out Vector3 hitPoint)
    {
        base.IsCollideWithCharacter(time, characterCollider, out hitPoint);
        Vector3 projectilePos = _DmgLine.GetPositionByTime(time);
        Vector3 projectilePosNext = _DmgLine.GetPositionByTime(time + Time.fixedDeltaTime);
        Vector3 characterPos = characterCollider.transform.position;
        // not even in damage segment range;
        if (Vector3.Dot(_SyncDirection, characterPos - projectilePos) < 0 
            || Vector3.Dot(_SyncDirection, characterPos - projectilePosNext) > 0)
        {
            return false;
        }


        if (true)
        {
            // unregister projectile here
            // 
            return true;
        }
        else
        {
            // do nothing, just return false;
            return false;
        }
    }
    
    public override float CalculateDamage(Vector3 hitPoint, CharacterBattleData character)
    {
        //return base.CalculateDamage(hitPoint, character);
        return .0f;
    }

    public void DisposeProjectile()
    {
        UnRegisterProjectile();
        GameObject.DestroyImmediate(this.gameObject);
    }
}
