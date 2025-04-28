using UnityEngine;

public class BossEnemyMoveState : BossEnemyState
{
    private Transform target;
    private readonly float rotationSpeed = 5f;

    public BossEnemyMoveState(BossEnemyFSM boss) : base(boss) { }

    public override void Enter()
    {
        target = boss.bossStatus.target;
        boss.Animator.SetBool("IsMoving", true);
    }

    public override void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(boss.transform.position, target.position);
        boss.Animator.SetFloat("DistanceToPlayer", distance);

        if (boss.CheckWallNearby())
        {
            boss.ChangeState(boss.patternState);
            return;
        }

        if (distance >= 6f)
        {
            MoveTowardsTarget();
            return;
        }

        if (distance > 3f && distance < 6f)
        {
            if (boss.patternController.IsFarPatternReady())
            {
                boss.ChangeState(boss.patternState);
                return;
            }
            else
            {
                MoveTowardsTarget();
                return;
            }
        }

        if (distance <= 3f)
        {
            boss.ChangeState(boss.patternState);
            return;
        }
    }

    private void MoveTowardsTarget()
    {
        Vector3 direction = (target.position - boss.transform.position).normalized;
        direction.y = 0f;

        boss.transform.position += direction * boss.bossStatus.moveSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(direction);
            boss.transform.rotation = Quaternion.Slerp(
                boss.transform.rotation,
                rot,
                Time.deltaTime * rotationSpeed
            );
        }
    }

    public override void Exit()
    {
        boss.Animator.SetBool("IsMoving", false);
    }
}
