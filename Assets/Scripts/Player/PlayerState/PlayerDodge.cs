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
        Vector3 direction = transform.forward;
        Vector3 target = transform.position + direction * dodgeDistance;
        StartCoroutine(DodgeMove(target));
    }

    private System.Collections.IEnumerator DodgeMove(Vector3 target)
    {
        float elapsed = 0f;
        Vector3 start = transform.position;
        while (elapsed < dodgeDuration)
        {
            transform.position = Vector3.Lerp(start, target, elapsed / dodgeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = target;
        stateMachine.ChangeState(PlayerState.Idle);
    }
}
