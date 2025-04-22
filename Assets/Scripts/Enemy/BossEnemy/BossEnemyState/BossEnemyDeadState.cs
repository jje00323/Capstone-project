using UnityEngine;

public class BossEnemyDeadState : BossEnemyState
{
    public BossEnemyDeadState(BossEnemyFSM boss) : base(boss) { }

    public override void Enter()
    {
        boss.Animator.SetTrigger("Die");
        boss.Animator.SetBool("IsMoving", false);
        boss.enabled = false;

        if (boss.TryGetComponent<Collider>(out var col))
            col.enabled = false;
    }

    public override void Update() { }

    public override void Exit() { }
}
