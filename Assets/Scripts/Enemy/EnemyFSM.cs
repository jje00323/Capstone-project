using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    public EnemyData enemyData;
    public EnemyStatus enemyStatus;
    public Animator animator;

    public EnemyState currentState;

    public EnemyIdleState idleState;
    public EnemyDetectState detectState;
    public EnemyAttackState attackState;
    public EnemyStunnedState stunnedState;
    public EnemyDeadState deadState;

    private void Start()
    {
        animator.runtimeAnimatorController = enemyData.animatorController;
        enemyStatus.Setup(enemyData);

        idleState = new EnemyIdleState(this);
        detectState = new EnemyDetectState(this);
        attackState = new EnemyAttackState(this);
        stunnedState = new EnemyStunnedState(this);
        deadState = new EnemyDeadState(this);

        ChangeState(idleState);
    }

    private void Update()
    {
        currentState?.Update();
    }

    public void ChangeState(EnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Die()
    {
        ChangeState(deadState);
        animator.SetTrigger("Die");
        Debug.Log("적 사망");
        // 이후 파괴 또는 풀 반환 처리
    }
}

