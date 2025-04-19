using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyPatternState : BossEnemyState
{
    private Transform target;

    public BossEnemyPatternState(BossEnemyFSM boss) : base(boss) { }

    public override void Enter()
    {
        target = boss.bossStatus.target;
    }

    public override void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(boss.transform.position, target.position);
        Vector3 dirToPlayer = (target.position - boss.transform.position).normalized;
        float angle = Vector3.Angle(boss.transform.forward, dirToPlayer);

        // 간단한 거리/방향 판단 후 공격
        if (distance > 5f)
        {
            boss.GetComponent<BossEnemyAttackController>()?.UseSpinAttack();
        }
        else if (angle <= 60f)
        {
            boss.GetComponent<BossEnemyAttackController>()?.UseSlashAttack();
        }
        else
        {
            boss.GetComponent<BossEnemyAttackController>()?.UseStompAttack();
        }
    }

    public override void Exit() { }
}
