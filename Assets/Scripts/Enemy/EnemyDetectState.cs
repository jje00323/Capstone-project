using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectState : EnemyState
{
    public EnemyDetectState(EnemyFSM enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.animator.SetBool("IsMoving", true);
    }

    public override void Update()
    {
        Transform target = enemy.enemyStatus.target;
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(enemy.transform.position, target.position);
        float distanceFromSpawn = Vector3.Distance(enemy.transform.position, enemy.spawnPosition);

        if (distanceToTarget < enemy.detectRadius && distanceFromSpawn < enemy.returnDistance)
        {
            // 방향 벡터 계산
            Vector3 dir = (target.position - enemy.transform.position).normalized;

            // 이동
            enemy.transform.position += dir * enemy.enemyData.moveSpeed * Time.deltaTime;

            // 부드럽게 회전
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, targetRotation, 360f * Time.deltaTime);

            // 공격 범위에 들어오면 공격 상태로 전환
            if (distanceToTarget < 2f)
            {
                enemy.ChangeState(enemy.attackState);
            }
        }
        else
        {
            enemy.ChangeState(enemy.idleState);
        }
    }


    public override void Exit() { }
}
