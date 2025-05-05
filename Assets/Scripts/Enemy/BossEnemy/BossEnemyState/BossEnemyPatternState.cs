using UnityEngine;
using System.Collections;

public class BossEnemyPatternState : BossEnemyState
{
    private Transform target;

    public BossEnemyPatternState(BossEnemyFSM boss) : base(boss) { }

    public override void Enter()
    {
        target = boss.bossStatus.target;
        boss.Animator.SetBool("IsMoving", false); // 공격 중 이동 금지
        boss.StartCoroutine(HandlePattern());
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

        int selectedIndex = boss.patternController.GetPattern(distance, isNearWall);

        if (selectedIndex == -1)
        {
            boss.ChangeState(boss.moveState);
            yield break;
        }

        boss.bossStatus.lastAttackIndex = selectedIndex; // 선택한 패턴 저장
        boss.Animator.SetInteger("AttackIndex", selectedIndex);

        if (selectedIndex >= 0 && selectedIndex <= 5) // Close 공격
        {
            //LockLookAtTargetOnce(); // 기존 LookAtTarget() → 회전 고정용으로 변경
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

        //boss.ChangeState(boss.idleState); // 임시
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

        // 1. 점프 직전 기준으로 플레이어 위치 계산 (한 번만)
        Vector3 targetPosition = target.position;
        Vector3 bossPosition = boss.transform.position;
        Vector3 direction = (targetPosition - bossPosition);
        direction.y = 0f;

        if (direction == Vector3.zero)
        {
            direction = boss.transform.forward; // 혹시라도 위치가 동일할 경우 보스 정면으로
        }

        direction.Normalize();

        // 2. 점프 목적지 설정: 플레이어보다 1.5m 짧은 거리
        float fullDistance = Vector3.Distance(bossPosition, targetPosition);
        float adjustedDistance = Mathf.Max(fullDistance - 1.5f, 0f);
        Vector3 destination = bossPosition + direction * adjustedDistance;

        // 3. 방향을 미리 회전시킴 (점프 시작 시점)
        boss.transform.rotation = Quaternion.LookRotation(direction);

        // 4. Animator 조건 설정
        boss.Animator.SetInteger("AttackIndex", 6);
        boss.Animator.SetTrigger("Attack");

        // 5. 애니메이션 상태 진입 대기
        yield return new WaitUntil(() =>
            boss.Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack_G")); // 정확한 상태 이름 확인 필요

        // 6. 점프 이동 (도중에 방향/타겟 변경 없음)
        float jumpTime = 0.85f;
        float elapsed = 0f;
        Vector3 startPos = boss.transform.position;

        while (elapsed < jumpTime)
        {
            float t = elapsed / jumpTime;
            Vector3 movePos = Vector3.Lerp(startPos, destination, t);
            movePos.y += Mathf.Sin(t * Mathf.PI) * 2f; // 포물선 효과

            boss.transform.position = movePos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 7. 최종 위치 보정
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

    public override void Update() 
    { 
        
    }
    public override void Exit() 
    {
        boss.lockRotation = false;
    }
}
