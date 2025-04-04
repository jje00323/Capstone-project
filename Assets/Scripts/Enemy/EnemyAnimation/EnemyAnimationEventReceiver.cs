using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEventReceiver : MonoBehaviour
{
    private EnemyFSM enemyFSM;

    private void Awake()
    {
        enemyFSM = GetComponent<EnemyFSM>();
    }

    // 애니메이션 이벤트로 호출됨
    public void OnAttackEnd()
    {
        if (enemyFSM.currentState is EnemyAttackState attackState)
        {
            attackState.OnAttackAnimationComplete();
        }
    }
}
