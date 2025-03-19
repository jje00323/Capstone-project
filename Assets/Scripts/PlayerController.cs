using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerControls controls;
    private NavMeshAgent agent; // NavMeshAgent 추가

    public bool isRightClickPressed = false;
    public JobAnimations animationManager; // 애니메이션 매니저 추가
    public Animator animator;
    private bool isMovementStopped = false;


    public float rotationSpeed = 20f; //ver2

    void Awake()
    {
        controls = new PlayerControls();
        animationManager = GetComponent<JobAnimations>(); //  자동으로 연결
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();



        agent.updateRotation = false; //ver2

        agent.speed = 5f;
        agent.acceleration = 999f;
        agent.autoBraking = false;
    }



    void OnEnable()
    {
        controls.Player.OnRightClick.performed += OnRightClickStart;
        controls.Player.OnRightClick.canceled += OnRightClickEnd;
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Player.OnRightClick.performed -= OnRightClickStart;
        controls.Player.OnRightClick.canceled -= OnRightClickEnd;
        controls.Disable();
    }

    void Update()
    {
        if (isRightClickPressed)
        {
            UpdateTargetPosition();
        }

        if (!isMovementStopped && isRightClickPressed)
        {
            UpdateTargetPosition();
        }

        if (agent.velocity.magnitude > 0.1f)
        {
            Vector3 direction = agent.steeringTarget - transform.position;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        //NavMeshAgent의 속도를 기준으로 애니메이션 처리
        if (agent.velocity.magnitude > 0.1f)
        {
            animationManager.PlayMovementAnimation(agent.velocity.magnitude);
        }
        else
        {
            animationManager.StopMovementAnimation();
        }

    }

    public void OnRightClickStart(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isRightClickPressed = true;
            UpdateTargetPosition();
        }
    }

    public void OnRightClickEnd(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            isRightClickPressed = false;
        }
    }

    private void UpdateTargetPosition()
    {
        if (!isRightClickPressed) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 2.0f, NavMesh.AllAreas))
            {
                agent.SetDestination(navHit.position);
            }
        }
    }

    public void StopMovement()
    {
        agent.ResetPath();
        agent.isStopped = true; // 이동 정지
        isMovementStopped = true;
        Debug.Log("PlayerController: 이동 멈춤");
    }

    public void StartMovement()
    {
        agent.isStopped = false; // 이동 다시 시작
        isMovementStopped = false;
        Debug.Log("PlayerController: 이동 시작");
    }

    public void StopAllMovementAnimations()
    {
        if (animator != null)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.type == AnimatorControllerParameterType.Float)
                {
                    animator.SetFloat(param.name, 0.1f);
                }
            }
            Debug.Log("모든 애니메이션 파라미터 0으로 설정");
        }
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

            RaycastHit hit;
            Vector3 leftFootPos = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
            Vector3 rightFootPos = animator.GetIKPosition(AvatarIKGoal.RightFoot);

            if (Physics.Raycast(leftFootPos + Vector3.up, Vector3.down, out hit, 1.5f, LayerMask.GetMask("Ground")))
            {
                animator.SetIKPosition(AvatarIKGoal.LeftFoot, hit.point);
            }

            if (Physics.Raycast(rightFootPos + Vector3.up, Vector3.down, out hit, 1.5f, LayerMask.GetMask("Ground")))
            {
                animator.SetIKPosition(AvatarIKGoal.RightFoot, hit.point);
            }
        }
    }
}
