using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    private bool isReturning = false;
    private float cooldownTimer = 0f;

    public EnemyIdleState(EnemyFSM enemy) : base(enemy) { }

    public override void Enter()
    {
        cooldownTimer = 0f;
        enemy.animator.SetBool("IsMoving", false);

        // 복귀 여부 판단
        float distanceFromSpawn = Vector3.Distance(enemy.transform.position, enemy.spawnPosition);
        isReturning = distanceFromSpawn > 0.5f;
    }

    public override void Update()
    {
        cooldownTimer += Time.deltaTime;

        Transform target = enemy.enemyStatus.target;
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(enemy.transform.position, target.position);
        float distanceFromSpawn = Vector3.Distance(enemy.transform.position, enemy.spawnPosition);

        // 쿨타임이 지난 후에만 탐지 전환 가능 (복귀 중이 아닐 때)
        if (!isReturning && cooldownTimer >= enemy.enemyData.attackCooldown)
        {
            if (distanceToTarget <= enemy.enemyData.detectRadius)
            {
                enemy.ChangeState(enemy.detectState);
                return;
            }
        }

        // 복귀 로직 (기존 구조 유지)
        if (isReturning)
        {
            if (distanceFromSpawn <= 0.5f)
            {
                isReturning = false;
                enemy.animator.SetBool("IsMoving", false);
                return;
            }

            enemy.animator.SetBool("IsMoving", true);

            Vector3 dir = (enemy.spawnPosition - enemy.transform.position).normalized;

            float moveSpeed = enemy.enemyData.moveSpeed * enemy.enemyData.returnSpeedMultiplier;
            enemy.transform.position += dir * moveSpeed * Time.deltaTime;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, targetRotation, 360f * Time.deltaTime);
        }
    }

    public override void Exit() { }
}
