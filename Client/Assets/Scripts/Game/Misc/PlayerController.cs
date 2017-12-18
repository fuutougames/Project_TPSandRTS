using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Here to control the Player
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBase
{
    private Vector3 velocity;
    private Vector3 heightCorrectedPoint;
    private Rigidbody myRigidbody;

    protected override void OnAwake()
    {
        base.OnAwake();
        myRigidbody = GetComponent<Rigidbody>();
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        myRigidbody.MovePosition(myRigidbody.position + velocity * Time.fixedDeltaTime);
    }

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void LookAt(Vector3 lookPoint)
    {
        heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectedPoint);
    }
}
