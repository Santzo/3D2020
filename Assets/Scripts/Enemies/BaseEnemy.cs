using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseEnemy : MonoBehaviour
{
    protected EnemyState enemyState = EnemyState.Patrol;
    protected NavMeshAgent agent;
    protected Animator animator;
    protected Vector3[] patrolPoints;
    protected int _walkAnimation, _idleAnimation, _attackAnimation, _walkMultiplier, currentPatrolPoint = -1;
    [SerializeField]
    protected float idleDelay = 1.5f;
    protected float currentIdleDelay = 0f;
    [SerializeField] protected float health = 3, moveSpeed = 4.5f;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        HashAnimations();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        var points = transform.Find("PatrolPoints");
        if (points != null)
        {
            patrolPoints = new Vector3[points.childCount];
            for (int i = 0; i < points.childCount; i++)
            {
                patrolPoints[i] = points.GetChild(i).position;
            }
            Debug.Log(patrolPoints.Length);
        }
        transform.position = patrolPoints[patrolPoints.Length - 1];
        transform.forward = patrolPoints[0] - transform.position;
    }

    protected void Update()
    {
        switch (enemyState)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Attack:
                Attack();
                break;
        }
    }

    protected abstract void Idle();
    protected abstract void Patrol();
    protected abstract void Attack();
    protected virtual void ChangePatrolPoint()
    {
        if (enemyState == EnemyState.Patrol)
        {
            ChangeState(EnemyState.Idle);
            currentPatrolPoint = currentPatrolPoint < patrolPoints.Length - 1 ? currentPatrolPoint += 1 : 0;
            agent.SetDestination(patrolPoints[currentPatrolPoint]);
        }
    }
    protected virtual void ChangeState(EnemyState newState)
    {
        enemyState = newState;

        if (enemyState == EnemyState.Idle)
        {
            currentIdleDelay = 0f;
            agent.isStopped = true;
            animator.SetBool(_walkAnimation, false);
        }
        else if (enemyState == EnemyState.Patrol)
        {
            agent.isStopped = false;
            animator.SetBool(_walkAnimation, true);
        }
    }

    protected virtual void HashAnimations()
    {
        _attackAnimation = Animator.StringToHash("Attack");
        _idleAnimation = Animator.StringToHash("Idle");
        _walkAnimation = Animator.StringToHash("Walk");
        _walkMultiplier = Animator.StringToHash("WalkMultiplier");
        animator.SetFloat(_walkMultiplier, moveSpeed * 0.5f);
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0) Die();
    }

    private void Die()
    {
        Debug.Log("Die");
    }
}
public enum EnemyState
{
    Idle,
    Patrol,
    Attack
}
