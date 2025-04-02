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

    [HideInInspector] public Vector3 spawnPosition;

    private void Start()
    {
        animator.runtimeAnimatorController = enemyData.animatorController;
        enemyStatus.Setup(enemyData);

        spawnPosition = transform.position;

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
        Debug.Log("Àû »ç¸Á");
    }

    private void OnDrawGizmosSelected()
    {
        if (enemyData != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, enemyData.detectRadius);
        }
    }

}
