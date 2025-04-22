using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyMoveState : BossEnemyState
{
    private Transform target;
    private float attackCooldownTimer = 0f;

    public BossEnemyMoveState(BossEnemyFSM boss) : base(boss) { }

    public override void Enter()
    {
        target = boss.bossStatus.target;
        boss.Animator.SetBool("IsMoving", true);
    }

    public override void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(boss.transform.position, target.position);
        boss.Animator.SetFloat("DistanceToPlayer", distance);

        Vector3 direction = (target.position - boss.transform.position).normalized;
        direction.y = 0f;

        // 1. 공격 조건 판단
        if (distance <= 6f)
        {
            if (boss.patternController.IsFarPatternReady())
            {
                boss.ChangeState(boss.patternState);
                return;
            }
            else if (distance <= 3f)
            {
                boss.ChangeState(boss.patternState);
                return;
            }
        }

        // 2. 이동
        boss.transform.position += direction * boss.bossStatus.moveSpeed * Time.deltaTime;
        boss.transform.rotation = Quaternion.Slerp(
            boss.transform.rotation,
            Quaternion.LookRotation(direction),
            Time.deltaTime * 5f
        );
    }

    public override void Exit()
    {
        boss.Animator.SetBool("IsMoving", false);
    }
}

