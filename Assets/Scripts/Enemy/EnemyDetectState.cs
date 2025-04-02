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

        float distance = Vector3.Distance(enemy.transform.position, target.position);

        // 공격 범위 안에 들어왔을 때 Attack으로 전환
        if (distance <= enemy.enemyData.attackRange)
        {
            // 공격 상태로 전환하여 회전 후 공격
            Debug.Log("플레이어 공격 범위로 들어옴");
            enemy.ChangeState(enemy.attackState);
            return;
        }

        // 탐지 범위 내에서는 계속 이동
        if (distance <= enemy.enemyData.detectRadius)
        {
            Vector3 dir = (target.position - enemy.transform.position).normalized;

            enemy.transform.position += dir * enemy.enemyData.moveSpeed * Time.deltaTime;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, targetRotation, 360f * Time.deltaTime);
        }
        else
        {
            enemy.ChangeState(enemy.idleState);
        }
    }

    public override void Exit()
    {
        enemy.animator.SetBool("IsMoving", false);
    }
}
