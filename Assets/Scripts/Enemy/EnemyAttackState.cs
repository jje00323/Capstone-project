using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private float attackCooldown = 1.5f;
    private float timer = 0f;

    public EnemyAttackState(EnemyFSM enemy) : base(enemy) { }

    public override void Enter()
    {
        timer = 0f;
        enemy.animator.SetTrigger("Attack");
    }

    public override void Update()
    {
        timer += Time.deltaTime;

        if (timer >= attackCooldown)
        {
            // 공격 애니메이션 다시 트리거
            timer = 0f;
            enemy.animator.SetTrigger("Attack");

            // 데미지 적용 예시 (PlayerStatus 가정)
            PlayerStatus player = enemy.enemyStatus.target.GetComponent<PlayerStatus>();
            if (player != null)
            {
                player.TakeDamage(enemy.enemyStatus.attackPower);
            }
        }

        float distance = Vector3.Distance(enemy.transform.position, enemy.enemyStatus.target.position);
        if (distance > 2.5f) // 추적 거리보다 멀어지면 다시 탐지 상태로
        {
            enemy.ChangeState(enemy.detectState);
        }
    }

    public override void Exit() { }
}

