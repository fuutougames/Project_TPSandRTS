using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// The enemy in the game
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State { Idle, Chasing, Attacking};
    private State currentState;

    private NavMeshAgent pathfinder;
    private Transform target;
    private LivingEntity targetEntity;
    private Material skinMaterial;
    private ParticleSystem deathEffect;
    public static event System.Action OnDeathStatic;

    private Color originalColor;

    private float attackDisanceThreshold = .5f;
    private float sqrDstToTarget = 0;
    private float timeBetweenAttacks = 1;
    private float nextAttackTime;
    private float myCollisionRadius;
    private float targetCollisionRadius;
    private bool hasTarget;
    private float damage = 1;

    protected override void OnAwake()
    {
        base.OnAwake();
        pathfinder = GetComponent<NavMeshAgent>();     
        deathEffect = ResourceManager.Instance.LoadResource<ParticleSystem>("Prefabs/EnemyDeathEffect");
        GameObject go_target = GameObject.FindGameObjectWithTag("Player");
        if (go_target != null)
        {
            hasTarget = true;
            target = go_target.transform;
            targetEntity = target.GetComponent<LivingEntity>();
            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        }
    }

    protected override void OnStart()
    {
        base.OnStart();

        if(hasTarget)
        {
            currentState = State.Chasing;
            targetEntity.OnDeath += OnTargetEntity;
            StartCoroutine(UpdatePath());
        }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (!hasTarget) return;
        if(Time.time > nextAttackTime)
        {
            sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
            if (sqrDstToTarget < Mathf.Pow(attackDisanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
            {
                nextAttackTime = Time.time + timeBetweenAttacks;
                AudioManager.Instance.PlaySound("EnemyAttack", transform.position);
                StartCoroutine(Attack());
            }
        }
    }

    private void OnTargetEntity()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColor)
    {
        pathfinder.speed = moveSpeed;
        if(hasTarget)
        {
            damage = Mathf.CeilToInt(targetEntity.startingHealth / hitsToKillPlayer);
        }
        startingHealth = enemyHealth;

        //deathEffect.GetComponent<ParticleSystemRenderer>().sharedMaterial.color = new Color(skinColor.r, skinColor.g, skinColor.b, 1);
        var main = deathEffect.main;
        main.startColor = new Color(skinColor.r, skinColor.g, skinColor.b, 0.9f);
        skinMaterial = GetComponent<Renderer>().material;
        skinMaterial.color = skinColor;
        originalColor = skinMaterial.color;
    }

    private Vector3 originalPosition;
    private Vector3 attackPosition;
    private float attackSpeed = 3;
    private float percent;
    IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathfinder.enabled = false;
        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false;

        originalPosition = transform.position;
        dirToTarget = (target.position - transform.position).normalized;
        attackPosition = target.position - dirToTarget * (myCollisionRadius);

        percent = 0;
        while(percent <= 1)
        {
            if(percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
            yield return null;
        }

        currentState = State.Chasing;
        pathfinder.enabled = true;
        skinMaterial.color = originalColor;
    }

    private float refreshRate = 0.25f;
    private Vector3 targetPositon = new Vector3();
    private Vector3 dirToTarget = new Vector3();
    private WaitForSeconds updateWait;
    IEnumerator UpdatePath()
    {
        updateWait = new WaitForSeconds(refreshRate);
        while (hasTarget)
        {
            if(currentState == State.Chasing)
            {
                if (!dead)
                {
                    dirToTarget = (target.position - transform.position).normalized;
                    targetPositon = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDisanceThreshold / 2);
                    pathfinder.SetDestination(targetPositon);
                }
            }
            yield return updateWait;
        }
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        AudioManager.Instance.PlaySound("Impact", transform.position);
        if(damage >= health)
        {
            if(OnDeathStatic != null)
            {
                OnDeathStatic();
            }
            AudioManager.Instance.PlaySound("EnemyDeath", transform.position);
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.main.startLifetimeMultiplier);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }
}
