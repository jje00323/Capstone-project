using System.Collections;
using UnityEngine;

public class EnemyStunnedState : EnemyState
{
    private float stunDuration = 1.5f;
    private float elapsed = 0f;

    public EnemyStunnedState(EnemyFSM enemy) : base(enemy) { }

    public override void Enter()
    {
        elapsed = 0f;

        // 애니메이터 초기화
        enemy.animator.ResetTrigger("Attack");
        enemy.animator.ResetTrigger("Die");
        enemy.animator.SetBool("IsMoving", false);

        // Stunned 트리거 설정
        enemy.animator.SetTrigger("Stunned");
    }

    public override void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= stunDuration)
        {
            enemy.animator.ResetTrigger("Stunned");
            enemy.ChangeState(enemy.idleState);
        }
    }

    public override void Exit()
    {
        // 이 상태에서는 animator.speed 조작 안 함
    }
}
