using UnityEngine;

public class BossEnemyMoveState : BossEnemyState
{
    private Transform target;
    private readonly float rotationSpeed = 3f;

    private float rotationCheckInterval = 0.3f;
    private float rotationTimer = 0f;
    private Vector3 cachedDirection = Vector3.zero;

    public BossEnemyMoveState(BossEnemyFSM boss) : base(boss) { }

    public override void Enter()
    {
        target = boss.bossStatus.target;
        boss.Animator.SetBool("IsMoving", true);

        // 처음 진입 시 즉시 회전
        if (target != null)
        {
            Vector3 lookDir = (target.position - boss.transform.position);
            lookDir.y = 0f;

            if (lookDir != Vector3.zero)
            {
                boss.transform.rotation = Quaternion.LookRotation(lookDir);
                cachedDirection = lookDir.normalized; // 처음 방향 저장
            }
        }

        rotationTimer = 0f; // 회전 타이머 초기화
    }

    public override void Update()
    {
        if (boss.currentState == boss.cutsceneState) return;  // 컷씬 중이면 동작 금지

        if (target == null) return;

        float distance = Vector3.Distance(boss.transform.position, target.position);
        boss.Animator.SetFloat("DistanceToPlayer", distance);

        if (boss.CheckWallNearby())
        {
            boss.ChangeState(boss.patternState);
            return;
        }

        if (distance <= 12f && boss.patternController.HasAvailablePattern(distance, boss.CheckWallNearby()))
        {
            boss.ChangeState(boss.patternState);
            return;
        }

        MoveTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        Vector3 direction = (target.position - boss.transform.position).normalized;
        direction.y = 0f;

        boss.transform.position += direction * boss.bossStatus.moveSpeed * Time.deltaTime;

        // 회전은 일정 간격마다 갱신된 방향으로만
        rotationTimer += Time.deltaTime;

        if (rotationTimer >= rotationCheckInterval)
        {
            rotationTimer = 0f;

            if (direction != Vector3.zero)
            {
                cachedDirection = direction; // 최신 방향 저장
            }
        }

        // 매 프레임 보간 회전 → cachedDirection 기준
        if (cachedDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cachedDirection);
            boss.transform.rotation = Quaternion.Slerp(
                boss.transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );
        }
    }

    public override void Exit()
    {
        boss.Animator.SetBool("IsMoving", false);
    }
}
