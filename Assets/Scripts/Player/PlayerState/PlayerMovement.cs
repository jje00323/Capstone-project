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

    private System.Action onArrivedCallback = null;
    private float stopDistanceBuffer = 0.2f;

    [Header("¿Ãµø ¿Ã∆Â∆Æ")]
    public GameObject moveClickEffectPrefab;

    private bool hasSpawnedEffect = false;
    private bool isRightClickActive = false;
    private float rightClickTimer = 0f;
    [SerializeField] private float rightClickThreshold = 0.1f;

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
    }

    void Update()
    {
        if (!stateMachine.CanMove()) return;

        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        bool isMoving = !agent.pathPending &&
                        agent.remainingDistance > agent.stoppingDistance &&
                        agent.velocity.sqrMagnitude > 0.05f;

        if (isMoving && stateMachine.CurrentState != PlayerState.Moving)
            stateMachine.ChangeState(PlayerState.Moving);
        else if (!isMoving && stateMachine.CurrentState != PlayerState.Idle)
            stateMachine.ChangeState(PlayerState.Idle);

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            isRightClickActive = true;
            rightClickTimer = 0f;
            HandleRightClick(true);
            hasSpawnedEffect = true;
        }
        else if (Mouse.current.rightButton.isPressed && isRightClickActive)
        {
            rightClickTimer += Time.deltaTime;
            if (rightClickTimer > rightClickThreshold)
                HandleRightClick(false);
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            isRightClickActive = false;
            hasSpawnedEffect = false;
        }

        RotateTowardsMovementDirection();
    }

    private void LateUpdate()
    {
        if (onArrivedCallback != null &&
            !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance + stopDistanceBuffer &&
            agent.velocity.sqrMagnitude < 0.1f)
        {
            StopAgent();
            onArrivedCallback?.Invoke();
            onArrivedCallback = null;
        }
    }

    public void MoveTo(Vector3 destination, float stoppingDistance, System.Action onArrived)
    {
        agent.stoppingDistance = stoppingDistance;
        agent.SetDestination(destination);
        onArrivedCallback = onArrived;
        ResumeAgent();
    }

    public void HandleRightClick(bool spawnEffect)
    {
        if (!stateMachine.CanMove()) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 desiredPoint = hit.point;

            if (NavMesh.SamplePosition(desiredPoint, out NavMeshHit navHit, 2.0f, NavMesh.AllAreas))
            {
                Vector3 targetPosition = navHit.position;
                agent.SetDestination(targetPosition);

                if (spawnEffect && !hasSpawnedEffect)
                    SpawnMoveEffect(targetPosition);
            }
            else
            {
                agent.ResetPath();
            }
        }
    }

    private void RotateTowardsMovementDirection()
    {
        Vector3 velocity = agent.velocity;
        velocity.y = 0f;

        if (velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    public void RotateToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
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

    void SpawnMoveEffect(Vector3 position)
    {
        if (moveClickEffectPrefab != null)
        {
            GameObject fx = Instantiate(moveClickEffectPrefab, position + Vector3.up * 0.1f, Quaternion.identity);
            fx.transform.localScale = Vector3.one * 0.7f;
            Destroy(fx, 2f);
        }
    }

    public void UpdateAnimatorReference(Animator newAnimator)
    {
        animator = newAnimator;
    }
}
