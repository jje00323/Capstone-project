using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyFSM enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.animator.SetBool("IsMoving", false);
    }

    public override void Update()
    {
        Transform target = enemy.enemyStatus.target;
        if (target == null) return;

        float distance = Vector3.Distance(enemy.transform.position, target.position);
        if (distance < 10f) // Å½Áö ¹üÀ§ ³»
        {
            enemy.ChangeState(enemy.detectState);
        }
    }

    public override void Exit() { }
}
