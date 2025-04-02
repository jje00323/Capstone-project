using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectState : EnemyState
{

    public EnemyDetectState(EnemyFSM enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.animator.SetBool("IsMoving", true);
        enemy.hasDetectedPlayer = true; // 탐지 시작
    }

    public override void Update()
    {
        Transform target = enemy.enemyStatus.target;
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(enemy.transform.position, target.position);
        float distanceFromSpawn = Vector3.Distance(enemy.transform.position, enemy.spawnPosition);

        // 1. 복귀 거리 초과 시 → 모든 것 무시하고 MoveState 진입
        if (distanceFromSpawn >= enemy.enemyData.returnDistance)
        {
            enemy.hasDetectedPlayer = false;
            enemy.animator.SetBool("IsMoving", true);
            enemy.moveState.SetDestination(enemy.spawnPosition, true); // 복귀
            enemy.ChangeState(enemy.moveState);
            return;
        }

        // 2. 공격 범위 안이면 공격
        if (distanceToTarget <= enemy.enemyData.attackRange)
        {
            enemy.animator.SetBool("IsMoving", false);
            enemy.ChangeState(enemy.attackState);
            return;
        }

        // 3. 그 외에는 계속 추적
        Vector3 dir = (target.position - enemy.transform.position).normalized;
        enemy.transform.position += dir * enemy.enemyData.moveSpeed * Time.deltaTime;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, targetRot, 360f * Time.deltaTime);
    }

    public override void Exit()
    {
        enemy.animator.SetBool("IsMoving", false);
    }
}
