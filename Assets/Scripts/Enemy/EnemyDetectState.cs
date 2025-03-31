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

        Vector3 dir = (target.position - enemy.transform.position).normalized;
        enemy.transform.position += dir * enemy.enemyData.moveSpeed * Time.deltaTime;
        enemy.transform.forward = dir;

        float distance = Vector3.Distance(enemy.transform.position, target.position);
        if (distance < 2f) // 공격 범위
        {
            enemy.ChangeState(enemy.attackState);
        }
    }

    public override void Exit() { }
}

