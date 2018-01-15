using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class SetUnitAniParams : Action
{
    public string aniparamsname;
    public ANIPARMTYPE paramType;
    public int param_int;
    public float param_float;
    public bool param_bool;
    private Animator _animator;

    public override void OnAwake()
    {
        base.OnAwake();
        _animator = this.transform.GetComponentInChildren<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        switch(paramType)
        {
            case ANIPARMTYPE.INT:
                _animator.SetInteger(aniparamsname, param_int);
                break;
            case ANIPARMTYPE.FLOAT:
                _animator.SetFloat(aniparamsname, param_float);
                break;
            case ANIPARMTYPE.BOOL:
                _animator.SetBool(aniparamsname, param_bool);
                break;
        }
        return TaskStatus.Success;
    }
}

public enum ANIPARMTYPE
{
    INT,
    FLOAT,
    BOOL,
}