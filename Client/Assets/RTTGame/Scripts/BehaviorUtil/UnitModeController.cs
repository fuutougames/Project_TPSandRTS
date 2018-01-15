using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks.Movement;

public enum UnitMode
{
    IDLE,
    AIM,
    MOVE,
    PATHMOVE,
    FOLLOW,
    PATROL,
    CHASE,
}

public class UnitModeController : MonoBehaviour
{
    private Dictionary<string, BehaviorTree> treeDic = new Dictionary<string, BehaviorTree>();
    public UnitMode _mode;

    private void Start()
    {
        var behaviorTrees = this.GetComponents<BehaviorTree>();
        for(int i = 0; i < behaviorTrees.Length; i++)
        {
            BehaviorTree bt = behaviorTrees[i];
            treeDic.Add(bt.BehaviorName, bt);
        }
        treeDic["Idle"].EnableBehavior();
    }

    private void UnitModeController_OnBehaviorEnd(Behavior behavior)
    {
        SwitchMode(UnitMode.AIM);
    }

    public void SwitchMode(UnitMode mode)
    {
        if (_mode == mode) return;

        // Disable Current Behavior
        switch(_mode)
        {
            case UnitMode.IDLE:
                treeDic["Idle"].DisableBehavior();
                break;
            case UnitMode.AIM:
                SetAimTarget(null);
                treeDic["Aim"].DisableBehavior();
                break;
            case UnitMode.MOVE:
                treeDic["Move"].DisableBehavior();
                break;
            case UnitMode.PATHMOVE:
                treeDic["PathMove"].DisableBehavior();
                break;
            case UnitMode.FOLLOW:
                SetFollowTarget(null);
                treeDic["Follow"].DisableBehavior();
                break;
            case UnitMode.PATROL:
                treeDic["Patrol"].DisableBehavior();
                break;
            case UnitMode.CHASE:
                treeDic["Chase"].DisableBehavior();
                break;
        }

        // Change current Behavior
        _mode = mode;

        // Enable Current Behavior
        switch(_mode)
        {
            case UnitMode.IDLE:
                treeDic["Idle"].EnableBehavior();
                break;
            case UnitMode.AIM:
                treeDic["Aim"].EnableBehavior();
                break;
            case UnitMode.MOVE:
                treeDic["Move"].EnableBehavior();
                break;
            case UnitMode.PATHMOVE:
                treeDic["PathMove"].EnableBehavior();
                PathMove pmTask = treeDic["PathMove"].FindTask<PathMove>();
                if (pmTask.waypoints != null)
                {
                    pmTask.waypoints.Value.Clear();
                }
                break;
            case UnitMode.FOLLOW:
                treeDic["Follow"].EnableBehavior();
                break;
            case UnitMode.PATROL:
                treeDic["Patrol"].EnableBehavior();
                break;
            case UnitMode.CHASE:
                treeDic["Chase"].EnableBehavior();
                break;
        }
    }

    public void SetAimTarget(GameObject target)
    {
        RotateTowards rotateTask = treeDic["Aim"].FindTask<RotateTowards>();
        if(rotateTask.target.Value != target)
        {
            treeDic["Aim"].DisableBehavior();
            treeDic["Aim"].EnableBehavior();
            rotateTask.target.Value = target;
        }       
    }

    public void SetMoveTarget(GameObject target)
    {
        if (_mode != UnitMode.MOVE) return;
        Seek seekTask = treeDic["Move"].FindTask<Seek>();
        seekTask.target.Value = target;
    }

    public void SetMovePath(GameObject target)
    {
        PathMove pmTask = treeDic["PathMove"].FindTask<PathMove>();
        pmTask.waypoints.Value.Add(target);
    }

    private SharedGameObject agent = new SharedGameObject();
    public void SetFollowTarget(GameObject target)
    {
        if (_mode != UnitMode.FOLLOW) return;

        Follow followTask = treeDic["Follow"].FindTask<Follow>();
        followTask.target.SetValue(target);

        RotateTowards rotateTask = treeDic["Follow"].FindTask<RotateTowards>();
        rotateTask.target.SetValue(target);

        WithinDistance withinTask = treeDic["Follow"].FindTask<WithinDistance>();
        withinTask.targetObject.SetValue(target);
    }
    
    public void SetPatrolPath(List<GameObject> path)
    {
        if (_mode != UnitMode.PATROL) return;

        Patrol partolTask = treeDic["Patrol"].FindTask<Patrol>();
        partolTask.waypoints.Value = path;
    }

    public void SetChaseTarget(GameObject target)
    {
        if (_mode != UnitMode.CHASE) return;
        Follow followTask = treeDic["Chase"].FindTask<Follow>();
        followTask.target.Value = target;
    }
}
