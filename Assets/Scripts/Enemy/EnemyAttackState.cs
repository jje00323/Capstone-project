using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private float attackCooldown = 1.2f;
    private float timer = 0f;
    private bool hasAttacked = false;

    public EnemyAttackState(EnemyFSM enemy) : base(enemy) { }

    public override void Enter()
    {
        timer = 0f;
        hasAttacked = false;

        // 공격 시작 시 한 번만 회전 (빠르게)
        Vector3 dir = (enemy.enemyStatus.target.position - enemy.transform.position).normalized;
        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, targetRotation, 480f); // 빠르게 맞춤
        }

        enemy.animator.SetTrigger("Attack");
    }


    public override void Update()
    {
        timer += Time.deltaTime;

        float distance = Vector3.Distance(enemy.transform.position, enemy.enemyStatus.target.position);
        if (distance > 2.5f)
        {
            enemy.ChangeState(enemy.detectState);
            return;
        }

        // 공격 딜 타이밍 (0.5초 후 한 번만 데미지 적용)
        if (!hasAttacked && timer >= 0.5f)
        {
            hasAttacked = true;

            PlayerStatus player = enemy.enemyStatus.target.GetComponent<PlayerStatus>();
            if (player != null)
            {
                player.TakeDamage(enemy.enemyStatus.attackPower);
            }
        }

        // 애니메이션 끝난 뒤 상태 복귀
        if (timer >= attackCooldown)
        {
            enemy.ChangeState(enemy.detectState);
        }
    }

    public override void Exit() { }
}


