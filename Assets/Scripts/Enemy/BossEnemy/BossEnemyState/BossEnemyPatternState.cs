using UnityEngine;
using System.Collections;
using System;

public class BossEnemyPatternState : BossEnemyState
{
    private Transform target;

    // 충돌 무시용 콜라이더 참조
    private Collider bossCollider;
    private Collider playerCollider;

    private bool isPhase2Started = false;
    //private Coroutine lightningRoutine = null;

    public BossEnemyPatternState(BossEnemyFSM boss) : base(boss) { }

    public override void Enter()
    {
        target = boss.bossStatus.target;
        boss.Animator.SetBool("IsMoving", false);

        if (bossCollider == null)
            bossCollider = boss.GetComponent<CapsuleCollider>();

        if (playerCollider == null)
            playerCollider = GameObject.FindWithTag("Player")?.GetComponent<Collider>();

        if (bossCollider != null && playerCollider != null)
            Physics.IgnoreCollision(bossCollider, playerCollider, true);

        // NavMeshObstacle 비활성화
        if (boss.navMeshObstacle != null)
        {
            boss.navMeshObstacle.enabled = false;
            Debug.Log("[PatternState] NavMeshObstacle → Disabled"); // 디버그 출력
        }
        else
        {
            Debug.LogWarning("[PatternState] NavMeshObstacle 컴포넌트가 없습니다.");
        }

        boss.StartCoroutine(HandlePattern());
    }

    public override void Exit()
    {
        boss.lockRotation = false;

        if (bossCollider != null && playerCollider != null)
            Physics.IgnoreCollision(bossCollider, playerCollider, false);

        // NavMeshObstacle 다시 활성화
        if (boss.navMeshObstacle != null)
            boss.navMeshObstacle.enabled = true;

        // 컷씬 예약 확인
        if (!boss.cutsceneTriggered && boss.cutsceneReserved)
        {
            boss.cutsceneTriggered = true;
            boss.cutsceneReserved = false;
            boss.ChangeState(boss.cutsceneState);
            Debug.Log("[PatternState] 컷씬 예약 실행 → CutsceneState 전환");
        }
    }
    public override void Update()
    {
        if (boss.cutsceneTriggered && !isPhase2Started)
        {
            isPhase2Started = true;
            boss.patternController.StartPhase2LightningPattern();
        }


    }
    private IEnumerator HandlePattern()
    {
        if (target == null)
        {
            boss.ChangeState(boss.idleState);
            yield break;
        }

        float distance = Vector3.Distance(boss.transform.position, target.position);
        bool isNearWall = boss.CheckWallNearby();

        //int selectedIndex = boss.patternController.GetPattern(distance, isNearWall);

        int selectedIndex;

        if (boss.useTestPattern)
        {
            selectedIndex = boss.testPatternIndex;
            Debug.Log($"[TEST] 강제 테스트 패턴 실행: {selectedIndex}");
        }
        else
        {
            selectedIndex = boss.patternController.GetPattern(distance, isNearWall);
        }

        // ^^^^ 강제 테스트 패턴 실행 구문 ^^^^

        if (selectedIndex == -1)
        {
            boss.ChangeState(boss.moveState);
            yield break;
        }

        boss.bossStatus.lastAttackIndex = selectedIndex; // 선택한 패턴 저장
        boss.Animator.SetInteger("AttackIndex", selectedIndex);

        // 속도 조절 메서드 호출
        ApplyAnimationSpeedForIndex(selectedIndex);

        if (selectedIndex >= 0 && selectedIndex <= 5) // Close 공격
        {
            yield return SmoothLookAtTarget(); // 회전 완료까지 기다림
            boss.Animator.SetTrigger("Attack");
        }
        else if (selectedIndex == 6) // Jump Attack
        {
            yield return ExecuteJumpAttack();
        }
        else if (selectedIndex == 7) // Spin Attack
        {
            ExecuteSpinAttack();
        }
        else if (selectedIndex == 8) // Wall Escape
        {
            LookAwayFromWall();
            boss.Animator.SetTrigger("Attack");
        }
    }

    private void LookAwayFromWall()
    {
        Vector3 forward = boss.transform.forward;
        Vector3 opposite = -forward;
        opposite.y = 0f;
        boss.transform.rotation = Quaternion.LookRotation(opposite);
    }

    private IEnumerator ExecuteJumpAttack()
    {
        if (target == null)
        {
            boss.ChangeState(boss.idleState);
            yield break;
        }

        Vector3 targetPosition = target.position;
        Vector3 bossPosition = boss.transform.position;
        Vector3 direction = (targetPosition - bossPosition);
        direction.y = 0f;

        if (direction == Vector3.zero)
        {
            direction = boss.transform.forward;
        }

        direction.Normalize();

        float fullDistance = Vector3.Distance(bossPosition, targetPosition);
        float adjustedDistance = Mathf.Max(fullDistance - 5.0f, 0f);
        Vector3 destination = bossPosition + direction * adjustedDistance;

        boss.transform.rotation = Quaternion.LookRotation(direction);

        boss.Animator.SetInteger("AttackIndex", 6);
        boss.Animator.SetTrigger("Attack");

        yield return new WaitUntil(() =>
            boss.Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack_G")); // 애니메이션 이름 확인 필요

        float jumpTime = 0.85f;
        float elapsed = 0f;
        Vector3 startPos = boss.transform.position;

        while (elapsed < jumpTime)
        {
            float t = elapsed / jumpTime;
            Vector3 movePos = Vector3.Lerp(startPos, destination, t);
            movePos.y += Mathf.Sin(t * Mathf.PI) * 0.2f; // 포물선 효과

            boss.transform.position = movePos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        boss.transform.position = new Vector3(destination.x, boss.transform.position.y, destination.z);
    }

    private void ExecuteSpinAttack()
    {
        if (target == null) return;

        Vector3 direction = (target.position - boss.transform.position).normalized;
        direction.y = 0f;
        if (direction == Vector3.zero) return;

        boss.transform.rotation = Quaternion.LookRotation(direction);
        boss.Animator.SetTrigger("Attack");
    }

    private IEnumerator SmoothLookAtTarget(float duration = 0.3f)
    {
        if (target == null) yield break;

        Vector3 dir = (target.position - boss.transform.position).normalized;
        dir.y = 0f;

        if (dir == Vector3.zero) dir = boss.transform.forward;

        Quaternion startRot = boss.transform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(dir);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            boss.transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        boss.transform.rotation = targetRot;
        boss.lockRotation = true;
    }
 
    private void ApplyAnimationSpeedForIndex(int index)
    {
        switch (index)
        {
            case 1:
                boss.Animator.speed = 0.65f;
                break;
            case 2:
                boss.Animator.speed = 0.65f;
                break;
            case 3:
                boss.Animator.speed = 0.75f;
                break;
            case 4:
                boss.Animator.speed = 0.75f;
                break;
            case 5:
                boss.Animator.speed = 0.75f;
                break;
            case 7:
                boss.Animator.speed = 0.75f;
                break;
            default:
                boss.Animator.speed = 1f;
                break;
        }
    }
    
}
