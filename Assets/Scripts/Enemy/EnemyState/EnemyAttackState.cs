using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private bool hasStartedAttack = false;
    private Quaternion targetRotation;

    public EnemyAttackState(EnemyFSM enemy) : base(enemy) { }

    public override void Enter()
    {
        hasStartedAttack = false;

        // 이 시점의 방향을 고정
        Vector3 dir = (enemy.enemyStatus.target.position - enemy.transform.position).normalized;
        if (dir != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(dir);
        }

        enemy.animator.SetBool("IsMoving", false);
    }

    public override void Update()
    {

        // 회전 완료 전이라면 계속 회전
        if (!hasStartedAttack)
        {
            enemy.transform.rotation = Quaternion.RotateTowards(
                enemy.transform.rotation,
                targetRotation,
                480f * Time.deltaTime
            );

            if (Quaternion.Angle(enemy.transform.rotation, targetRotation) < 1f)
            {
                hasStartedAttack = true;
                Debug.Log("플레이어 공격");
                enemy.animator.SetTrigger("Attack");
            }

            return;
        }

    }

    public override void Exit() { }

    public void OnAttackAnimationComplete()
    {
        if (enemy.currentState == this)
        {
            enemy.ChangeState(enemy.idleState);
        }
    }
}
