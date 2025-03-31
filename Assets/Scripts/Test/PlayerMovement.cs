using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using static PlayerStateMachine;

[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private PlayerStateMachine stateMachine;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        stateMachine = GetComponent<PlayerStateMachine>();

        agent = GetComponent<NavMeshAgent>();



        agent.updateRotation = false;

        agent.speed = 5f;
        agent.acceleration = 999f;
        agent.autoBraking = false;
    }

    void Update()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed); // 이건 항상 실행

        bool isMoving = !agent.pathPending &&
                        agent.remainingDistance > agent.stoppingDistance &&
                        agent.velocity.sqrMagnitude > 0.05f;

        // FSM 상태가 Attacking일 땐 상태 전환 막기만!
        if (stateMachine.CurrentState != PlayerState.Attacking)
        {
            if (isMoving && stateMachine.CurrentState != PlayerState.Moving)
            {
                stateMachine.ChangeState(PlayerState.Moving);
            }
            else if (!isMoving && stateMachine.CurrentState != PlayerState.Idle)
            {
                stateMachine.ChangeState(PlayerState.Idle);
            }
        }

        if (stateMachine.CanMove() && Mouse.current.rightButton.isPressed)
        {
            HandleRightClick();
        }

        if (stateMachine.CurrentState != PlayerState.Attacking)
        {
            RotateTowardsMovementDirection();
        }
    }

    public void HandleRightClick()
    {
        if (!stateMachine.CanMove()) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            agent.SetDestination(hit.point);
        }
    }

    private void RotateTowardsMovementDirection()
    {
        Vector3 velocity = agent.velocity;
        velocity.y = 0f; // 수직 회전 방지

        if (velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    // 플레이어 공격 시 캐릭터 회전
    public void RotateToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 lookPoint = hit.point;
            lookPoint.y = transform.position.y; // y값 고정해서 수평 회전만

            Vector3 direction = (lookPoint - transform.position).normalized;
            if (direction.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
    public void StopAgent()
    {
        agent.isStopped = true;
        agent.ResetPath(); // 이동 중이던 경로 제거
    }

    public void ResumeAgent()
    {
        agent.isStopped = false;
    }
}
