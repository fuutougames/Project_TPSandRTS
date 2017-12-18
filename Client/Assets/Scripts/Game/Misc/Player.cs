using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Player in the game
/// </summary>
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public float moveSpeed = 5;
    private Crosshairs crosshairs;

    private Vector3 moveInput;
    private Vector3 moveVelocity;
    private Ray ray;
    private Plane groundPlane;
    private float rayDistance;
    private Vector3 hitPoint;
    private PlayerController controller;
    private GunController gunController;
    private Camera viewCamera;

    protected override void OnAwake()
    {
        base.OnAwake();
        controller = this.GetComponent<PlayerController>();
        gunController = this.GetComponent<GunController>();
        viewCamera = Camera.main;
        crosshairs = FindObjectOfType<Crosshairs>();
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    protected override void OnStart()
    {
        base.OnStart();
        gunController.EquipGun(1);
        groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
    }

    private void OnNewWave(int waveNumber)
    {
        health = startingHealth;
        gunController.EquipGun(waveNumber - 1);
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
        if(groundPlane.Raycast(ray, out rayDistance))
        {
            hitPoint = ray.GetPoint(rayDistance);
            //Debug.DrawLine(ray.origin, hitPoint, Color.red);
            controller.LookAt(hitPoint);
            crosshairs.transform.position = hitPoint;
            crosshairs.DetectTargets(ray);

            //print((new Vector2(hitPoint.x, hitPoint.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude);
            if((new Vector2(hitPoint.x, hitPoint.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 2.5f)
            {
                gunController.Aim(hitPoint);
            }
        }

        // Weapon input
        if(Input.GetMouseButton(0))
        {
            gunController.OnTriggerHold();
        }

        if(Input.GetMouseButtonUp(0))
        {
            gunController.OnTriggerRelease();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            gunController.Reload();
        }

        if(transform.position.y < -10)
        {
            TakeDamage(health);
        }
    }

    protected override void Die()
    {
        AudioManager.Instance.PlaySound("PlayerDeath", this.transform.position);
        base.Die();
    }
}