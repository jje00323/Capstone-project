using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeadState : EnemyState
{
    public EnemyDeadState(EnemyFSM enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.animator.SetTrigger("Die");
        // 움직임 중지, 충돌 비활성화 등
        enemy.GetComponent<Collider>().enabled = false;
        enemy.enabled = false;
    }

    public override void Update() { }

    public override void Exit() { }
}
