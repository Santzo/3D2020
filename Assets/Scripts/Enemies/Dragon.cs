using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : BaseEnemy
{
    protected override void Idle()
    {
        currentIdleDelay += Time.deltaTime;
        if (currentIdleDelay > idleDelay)
        {
            ChangeState(EnemyState.Patrol);
        }
    }
    protected override void Patrol()
    {
        animator.SetBool(_walkAnimation, true);
        if (!agent.hasPath && !agent.pathPending || agent.remainingDistance == 0)
        {
            ChangePatrolPoint();
        }
    }
    protected override void Attack()
    {

    }
}
