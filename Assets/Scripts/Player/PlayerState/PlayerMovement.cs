using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static PlayerStateMachine;

[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private PlayerStateMachine stateMachine;

    private System.Action onArrivedCallback = null;
    private float stopDistanceBuffer = 0.2f;

    private float defaultStoppingDistance = 0.5f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        stateMachine = GetComponent<PlayerStateMachine>();

        agent.updateRotation = false;
        agent.speed = 5f;
        agent.acceleration = 999f;
        agent.autoBraking = false;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        defaultStoppingDistance = agent.stoppingDistance;
    }

    void Update()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        bool isMoving = !agent.pathPending &&
                        agent.remainingDistance > agent.stoppingDistance &&
                        agent.velocity.sqrMagnitude > 0.05f;

        if (stateMachine.CurrentState != PlayerState.Attacking &&
            stateMachine.CurrentState != PlayerState.SkillCasting &&
            stateMachine.CurrentState != PlayerState.Dodging)
        {
            if (isMoving && stateMachine.CurrentState != PlayerState.Moving)
                stateMachine.ChangeState(PlayerState.Moving);
            else if (!isMoving && stateMachine.CurrentState != PlayerState.Idle)
                stateMachine.ChangeState(PlayerState.Idle);
        }
        

        RotateTowardsMovementDirection();
    }

    private void LateUpdate()
    {
        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance + stopDistanceBuffer &&
            agent.velocity.sqrMagnitude < 0.1f)
        {
            StopAgent();
            onArrivedCallback?.Invoke();
            onArrivedCallback = null;

            agent.ResetPath();
        }
    }

    public void MoveTo(Vector3 destination, float stoppingDistance, System.Action onArrived)
    {
        agent.stoppingDistance = stoppingDistance;
        agent.SetDestination(destination);
        onArrivedCallback = onArrived;
        ResumeAgent();
    }

  

    public void RotateTowardsMovementDirection()
    {
        Vector3 velocity = agent.velocity;
        velocity.y = 0f;

        if (velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    public void RotateToPosition(Vector3 worldPosition)
    {
        Vector3 direction = (worldPosition - transform.position);
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;
        }
    }

    public void RotateToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 lookPoint = hit.point;
            lookPoint.y = transform.position.y;

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
        agent.ResetPath();
    }

    public void ResumeAgent()
    {
        agent.isStopped = false;
    }


    public void ResetStoppingDistance()
    {
        agent.stoppingDistance = defaultStoppingDistance;
    }

    public void UpdateAnimatorReference(Animator newAnimator)
    {
        animator = newAnimator;
    }

    public bool IsAgentMoving()
    {
        return agent.pathPending || agent.remainingDistance > agent.stoppingDistance + 0.1f;
    }

    public bool GetAgentStoppedStatus()
    {
        return agent.isStopped;
    }


    public void ShowSkillRangeIndicator(Vector3 center, float radius)
    {
        Debug.DrawLine(center, center + Vector3.up * 3, Color.red, 2f);
    }

    public void HideSkillRangeIndicator()
    {
    }
}