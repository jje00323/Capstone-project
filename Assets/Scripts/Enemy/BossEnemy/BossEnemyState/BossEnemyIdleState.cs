using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyIdleState : BossEnemyState
{
    public BossEnemyIdleState(BossEnemyFSM boss) : base(boss) { }

    public override void Enter()
    {
        boss.animator.SetTrigger("Idle");
    }

    public override void Update()
    {
        // 바로 전투 시작 (졸업작품이므로 단순화)
        boss.ChangeState(boss.patternState);
    }

    public override void Exit() { }
}