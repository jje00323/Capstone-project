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
    private bool hasSpawnedEffect = false;

    [Header("이동 이펙트")]
    public GameObject moveClickEffectPrefab;

    [SerializeField] private float rootMotionMultiplier = 1.5f;

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float approachDistance = 1.5f;
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

        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
    }

    void Update()
    {
        if (!stateMachine.CanMove())
            return;

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

        if (stateMachine.CanMove())
        {
            if (Mouse.current.rightButton.wasPressedThisFrame && !hasSpawnedEffect)
            {
                HandleRightClick();                 // 이동 처리 + 이펙트 생성
                hasSpawnedEffect = true;
            }
            else if (Mouse.current.rightButton.isPressed)
            {
                HandleRightClick();                 // 이동 처리만 (이펙트는 X)
            }

            if (Mouse.current.rightButton.wasReleasedThisFrame)
            {
                hasSpawnedEffect = false;          // 클릭 끝나면 다시 초기화
            }
        }


        if (stateMachine.CurrentState != PlayerState.Attacking)
        {
            RotateTowardsMovementDirection();
        }



        //CheckAgentStuck();
    }

    public void UpdateAnimatorReference(Animator newAnimator)
    {
        animator = newAnimator;
    }

    public void HandleRightClick()
    {
        if (!stateMachine.CanMove()) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            bool isEnemy = ((1 << hit.collider.gameObject.layer) & enemyLayer) != 0;

            if (isEnemy && hit.collider.CompareTag("Enemy"))
            {
                Vector3 enemyPos = hit.collider.transform.position;
                float currentDistance = Vector3.Distance(transform.position, enemyPos);

                if (currentDistance < 1.2f) // 겹쳐 있는 경우만
                {
                    Vector3 dir = transform.forward;
                    Vector3 escapePos = enemyPos - dir * approachDistance;

                    if (NavMesh.SamplePosition(escapePos, out NavMeshHit navHit, 2.0f, NavMesh.AllAreas))
                    {
                        agent.SetDestination(navHit.position);
                    }
                }
                else
                {
                    // 겹쳐 있지 않으면 제자리 유지
                    agent.ResetPath();
                }
            }
            else
            {
                agent.SetDestination(hit.point); // 일반 이동
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                SpawnMoveEffect(hit.point);
            }
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

    void SpawnMoveEffect(Vector3 position)
    {
        if (moveClickEffectPrefab != null)
        {
            GameObject fx = Instantiate(moveClickEffectPrefab, position + Vector3.up * 0.1f, Quaternion.identity);

            fx.transform.localScale = Vector3.one * 0.7f;
            Destroy(fx, 2f); // 1.5초 뒤 제거
        }
    }
    void OnAnimatorMove()
    {
        if (stateMachine.CurrentState == PlayerStateMachine.PlayerState.SkillCasting ||
            stateMachine.CurrentState == PlayerStateMachine.PlayerState.Attacking)
        {
            Vector3 delta = animator.deltaPosition * rootMotionMultiplier;
            delta.y = 0f; // 수직 이동 제거
            transform.position += delta;

            transform.rotation *= animator.deltaRotation;
        }
    }

    private Vector3 GetClosestPointNearEnemy(Vector3 enemyPos)
    {
        Vector3 dir = (transform.position - enemyPos).normalized;
        Vector3 desiredPos = enemyPos + dir * approachDistance;

        if (NavMesh.SamplePosition(desiredPos, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            return hit.position;

        return transform.position; // fallback
    }

}
