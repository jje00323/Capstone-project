using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM : BaseEnemyFSM
{
    public bool hasDetectedPlayer = false;

    public EnemyData enemyData;
    public EnemyStatus enemyStatus;
    public Animator animator;

    public EnemyState currentState;

    public EnemyIdleState idleState;
    public EnemyMoveState moveState;
    public EnemyDetectState detectState;
    public EnemyAttackState attackState;
    public EnemyStunnedState stunnedState;
    public EnemyDeadState deadState;

    [HideInInspector] public Vector3 spawnPosition;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (animator != null)
        {
            animator.runtimeAnimatorController = enemyData.animatorController;
        }

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
        moveState = new EnemyMoveState(this);
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
        if (currentState == newState) return;

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public override void Die()
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

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(spawnPosition, enemyData.returnDistance);
        }
    }
}

