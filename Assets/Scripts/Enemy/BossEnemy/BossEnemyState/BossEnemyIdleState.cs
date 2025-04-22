using UnityEngine;

public class BossEnemyIdleState : BossEnemyState
{
    private float delay;
    private float timer;

    public BossEnemyIdleState(BossEnemyFSM boss) : base(boss) { }

    public override void Enter()
    {
        boss.Animator.SetTrigger("Idle");
        delay = Random.Range(0.5f, 1.5f);
        timer = 0f;
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer >= delay)
        {
            boss.ChangeState(boss.moveState);
        }
    }

    public override void Exit() { }
}
