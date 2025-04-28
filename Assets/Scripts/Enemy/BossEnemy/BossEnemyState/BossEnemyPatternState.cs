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

        boss.bossStatus.lastAttackIndex = selectedIndex; // ★ 선택한 패턴 저장
        boss.Animator.SetInteger("AttackIndex", selectedIndex);

        if (selectedIndex >= 0 && selectedIndex <= 5) // Close 공격
        {
            LookAtTarget();
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

        boss.ChangeState(boss.idleState); // 임시
    }

    private void LookAtTarget()
    {
        if (target == null) return;

        Vector3 direction = (target.position - boss.transform.position).normalized;
        direction.y = 0f;
        if (direction == Vector3.zero) return;

        boss.transform.rotation = Quaternion.LookRotation(direction);
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

        Vector3 startPos = boss.transform.position;
        Vector3 destination = new Vector3(target.position.x, startPos.y, target.position.z);

        boss.Animator.SetTrigger("Attack"); // Attack 트리거 먼저 발동

        float jumpTime = 0.85f;
        float elapsed = 0f;

        while (elapsed < jumpTime)
        {
            float t = elapsed / jumpTime;
            Vector3 movePos = Vector3.Lerp(startPos, destination, t);
            movePos.y += Mathf.Sin(t * Mathf.PI) * 2f;

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

    public override void Update() 
    { 
        
    }
    public override void Exit() { }
}
