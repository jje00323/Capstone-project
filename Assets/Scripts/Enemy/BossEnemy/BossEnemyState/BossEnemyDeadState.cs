using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyDeadState : BossEnemyState
{
    public BossEnemyDeadState(BossEnemyFSM boss) : base(boss) { }

    public override void Enter()
    {
        boss.animator.SetTrigger("Die");
        boss.enabled = false;
        boss.GetComponent<Collider>().enabled = false;
    }

    public override void Update() { }

    public override void Exit() { }
}
