using UnityEngine;
using System.Collections;

public class BossEnemyPatternState : BossEnemyState
{
    private Transform target;

    public BossEnemyPatternState(BossEnemyFSM boss) : base(boss) { }

    public override void Enter()
    {
        target = boss.bossStatus.target;
        boss.StartCoroutine(HandlePattern());
    }

    private IEnumerator HandlePattern()
    {
        float distance = Vector3.Distance(boss.transform.position, target.position);
        bool isNearWall = boss.CheckWallNearby();

        int selectedIndex = boss.patternController.GetPattern(distance, isNearWall);

        if (selectedIndex == -1)
        {
            boss.ChangeState(boss.idleState);
            yield break;
        }

        boss.Animator.SetInteger("AttackIndex", selectedIndex);
        boss.Animator.SetTrigger("Attack");

        if (selectedIndex == 6)
        {
            yield return boss.StartCoroutine(ExecuteJumpAttack());
            yield break;
        }
        else if (selectedIndex == 7)
        {
            ExecuteSpinAttack();
            yield break;
        }
        else if (selectedIndex == 8)
        {
            yield return boss.StartCoroutine(HandleRotationDelay(0.5f, 1.5f));
        }
        else
        {
            bool isLight = selectedIndex >= 0 && selectedIndex <= 2;
            float delay = isLight ?
                Random.Range(0.5f, 1f) :
                Random.Range(1.5f, 2.5f);

            yield return boss.StartCoroutine(HandleRotationDelay(delay, delay));
        }

        boss.ChangeState(boss.idleState);
    }

    private IEnumerator HandleRotationDelay(float minTime, float maxTime)
    {
        float delay = Random.Range(minTime, maxTime);
        float elapsed = 0f;

        boss.Animator.SetBool("IsMoving", true);

        while (elapsed < delay)
        {
            if (target != null)
            {
                Vector3 dir = (target.position - boss.transform.position).normalized;
                dir.y = 0f;

                Quaternion targetRot = Quaternion.LookRotation(dir);
                boss.transform.rotation = Quaternion.Slerp(
                    boss.transform.rotation,
                    targetRot,
                    Time.deltaTime * 5f
                );
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        boss.Animator.SetBool("IsMoving", false);
    }

    private IEnumerator ExecuteJumpAttack()
    {
        Vector3 startPos = boss.transform.position;
        Vector3 destination = new Vector3(target.position.x, startPos.y, target.position.z);
        float jumpTime = 0.85f; // 점프 이동 시간 (0~51프레임 @ 60FPS 기준)

        float elapsed = 0f;

        while (elapsed < jumpTime)
        {
            float t = elapsed / jumpTime;
            Vector3 movePos = Vector3.Lerp(startPos, destination, t);
            movePos.y += Mathf.Sin(t * Mathf.PI) * 2f;

            boss.transform.position = movePos;

            Quaternion targetRot = Quaternion.LookRotation(destination - boss.transform.position);
            boss.transform.rotation = Quaternion.Slerp(boss.transform.rotation, targetRot, Time.deltaTime * 7f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        boss.transform.position = destination;

        float delay = Random.Range(0.5f, 1.5f);
        yield return new WaitForSeconds(delay);

        boss.ChangeState(boss.idleState);
    }

    private void ExecuteSpinAttack()
    {
        if (target == null) return;

        Vector3 dir = (target.position - boss.transform.position).normalized;
        dir.y = 0;

        Quaternion rot = Quaternion.LookRotation(dir);
        boss.transform.rotation = rot;

        boss.StartCoroutine(SpinAttackEndDelay());

        // 애니메이션 트리거는 Enter에서 이미 호출됨
    }

    private IEnumerator SpinAttackEndDelay()
    {
        float animDuration = 2.95f; // Spin Attack 애니메이션 길이
        yield return new WaitForSeconds(animDuration);

        float delay = Random.Range(0.5f, 1.5f);
        yield return new WaitForSeconds(delay);

        boss.ChangeState(boss.idleState);
    }

    public override void Update() { }

    public override void Exit() { }
}
