using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The bullet base class
/// </summary>
public class Projectile : MonoBase
{
    public LayerMask collisionMask;
    public Color trailColor;
    private float speed = 10;
    private float moveDistance;
    private float damage = 1;
    private float lifetime = 3;
    private float skinWidth = .1f;

    protected override void OnStart()
    {
        base.OnStart();
        Destroy(gameObject, 3);
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
        if(initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0], transform.position);
        }

        GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailColor);
    }

    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        moveDistance = Time.fixedDeltaTime * speed;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
    }

    private Ray ray;
    private RaycastHit hit;
    void CheckCollisions(float _moveDistance)
    {
        ray = new Ray(transform.position, transform.forward);
        if(Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit.collider, hit.point);
        }
    }

    void OnHitObject(Collider c, Vector3 hitPoint)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage, hitPoint, transform.forward);
        }
        GameObject.Destroy(gameObject);
    }
}