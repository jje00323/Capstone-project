using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    private bool isReturning = false;

    public EnemyIdleState(EnemyFSM enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.animator.SetBool("IsMoving", false);

        // 복귀 여부 판단
        float distanceFromSpawn = Vector3.Distance(enemy.transform.position, enemy.spawnPosition);
        isReturning = distanceFromSpawn > 0.5f;
    }

    public override void Update()
    {
        Transform target = enemy.enemyStatus.target;
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(enemy.transform.position, target.position);
        float distanceFromSpawn = Vector3.Distance(enemy.transform.position, enemy.spawnPosition);

        // 플레이어가 탐지 범위에 있으면 다시 탐지 상태로 전환
        if (distanceToTarget < enemy.detectRadius && !isReturning)
        {
            enemy.ChangeState(enemy.detectState);
            return;
        }

        // 복귀 로직
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

            // 복귀 시 속도 증가
            float moveSpeed = enemy.enemyData.moveSpeed * enemy.returnSpeedMultiplier;
            enemy.transform.position += dir * moveSpeed * Time.deltaTime;

            // 부드럽게 회전
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, targetRotation, 360f * Time.deltaTime);
        }
    }

    public override void Exit() { }
}
