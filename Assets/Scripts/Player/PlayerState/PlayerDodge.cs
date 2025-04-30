using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerStateMachine;

[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerDodge : MonoBehaviour
{
    private PlayerStateMachine stateMachine;
    private Animator animator;

    public float dodgeDistance = 5f;
    public float dodgeDuration = 0.2f;

    void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        animator = GetComponent<Animator>();
    }

    public void HandleDodge()
    {
        if (!stateMachine.CanDodge()) return;

        stateMachine.ChangeState(PlayerState.Dodging);
        animator.SetTrigger("Dodge");

        // RootMotion은 애니메이션 클립 설정에 따라 적용됨 (코드에서는 제어하지 않음)
    }

    // 애니메이션 이벤트로 호출될 종료 시점 처리 함수
    public void EndDodge()
    {
        stateMachine.ChangeState(PlayerState.Idle);
    }
}