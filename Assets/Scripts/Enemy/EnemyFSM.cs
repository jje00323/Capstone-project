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

    public float detectRadius = 8f;
    [HideInInspector] public Vector3 spawnPosition; // 적의 원래 위치
    public float returnSpeedMultiplier = 2f;
    public float returnDistance = 15f; // 원위치로 돌아가는 최대 거리

    private void Start()
    {
        animator.runtimeAnimatorController = enemyData.animatorController;
        enemyStatus.Setup(enemyData);

        spawnPosition = transform.position; // 스폰 위치 저장

        if (enemyStatus.target == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                enemyStatus.target = playerObj.transform;
            }
        }

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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}

