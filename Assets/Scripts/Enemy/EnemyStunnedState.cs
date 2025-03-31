using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyStunnedState : EnemyState
{
    private float stunDuration = 1.5f;
    private float elapsed = 0f;

    public EnemyStunnedState(EnemyFSM enemy) : base(enemy) { }

    public override void Enter()
    {
        elapsed = 0f;
        enemy.animator.SetTrigger("Stunned");
    }

    public override void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= stunDuration)
        {
            enemy.ChangeState(enemy.idleState);
        }
    }

    public override void Exit() { }
}
