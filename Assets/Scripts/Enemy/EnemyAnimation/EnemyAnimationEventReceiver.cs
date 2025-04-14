using System.Collections;
using UnityEngine;

public class EnemyAnimationEventReceiver : MonoBehaviour
{
    private EnemyFSM enemyFSM;
    private Animator animator;

    private void Awake()
    {
        enemyFSM = GetComponent<EnemyFSM>();
        animator = GetComponent<Animator>();
    }

    // 애니메이션 이벤트로 호출됨 (공격 종료 시)
    public void OnAttackEnd()
    {
        if (enemyFSM.currentState is EnemyAttackState attackState)
        {
            attackState.OnAttackAnimationComplete();
        }
    }

    // Stunned 애니메이션 중간에 호출됨
    public void TriggerStunPause()
    {
        StartCoroutine(StunPauseEffect());
    }

    private IEnumerator StunPauseEffect()
    {
        animator.speed = 0f;
        yield return new WaitForSeconds(0.25f);
        animator.speed = 1f;
    }
}
