using UnityEngine;

public class BossEnemyIdleState : BossEnemyState
{
    private Transform target;
    private float delay;
    private float timer;

    public BossEnemyIdleState(BossEnemyFSM boss) : base(boss) { }

    public override void Enter()
    {
        target = boss.bossStatus.target;

        boss.Animator.SetBool("IsMoving", false);
        boss.Animator.SetTrigger("Idle");

        delay = GetDelayBasedOnAttackIndex(boss.bossStatus.lastAttackIndex);
        timer = 0f;
    }

    private float GetDelayBasedOnAttackIndex(int index)
    {
        if (index >= 0 && index <= 2) // 가벼운 공격
            return Random.Range(1.0f, 2.0f);
        else if (index >= 3 && index <= 5) // 콤보 공격
            return Random.Range(2.0f, 4.0f);
        else if (index == 6 || index == 7) // 점프/스핀 공격
            return Random.Range(3.0f, 4.0f);
        else if (index == 8) // 벽 탈출
            return Random.Range(1.0f, 2.0f);
        else // 예외처리
            return Random.Range(1.0f, 2.0f);
    }

    public override void Update()
    {
        if (target == null) return;

        timer += Time.deltaTime;

        float distance = Vector3.Distance(boss.transform.position, target.position);
        boss.Animator.SetFloat("DistanceToPlayer", distance);

        if (timer >= delay)
        {

            bool isNearWall = boss.CheckWallNearby();

            if (isNearWall)
            {
                boss.ChangeState(boss.patternState);
                return;
            }

            if (distance >= 6f)
            {
                boss.ChangeState(boss.moveState);
                return;
            }
            else
            {
                boss.ChangeState(boss.patternState);
                return;
            }
        }
    }

    public override void Exit()
    {
        boss.Animator.ResetTrigger("Idle");
    }
}
