using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class BasementPlayer : MonoBase
{
    public float moveSpeed = 5;
    private Vector3 moveInput;
    private Vector3 moveVelocity;
    private Ray ray;
    private Camera viewCamera;

    private PlayerController controller;
    private Crosshairs crosshairs;
    private Plane groundPlane;
    private float rayDistance;
    private Vector3 hitPoint;

    protected override void OnAwake()
    {
        base.OnAwake();
        controller = this.GetComponent<PlayerController>();
        crosshairs = FindObjectOfType<Crosshairs>();
        viewCamera = Camera.main;

    }

    protected override void OnStart()
    {
        base.OnStart();
        groundPlane = new Plane(Vector3.up, Vector3.up);

    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        // Movement input
        moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        // Look input  
        ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            hitPoint = ray.GetPoint(rayDistance);
            //Debug.DrawLine(ray.origin, hitPoint, Color.red);
            controller.LookAt(hitPoint);
            crosshairs.transform.position = hitPoint;
            crosshairs.DetectTargets(ray);
        }
    }
}
