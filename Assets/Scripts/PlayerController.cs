using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerControls controls;
    private Vector3 targetPosition;
    private bool isMoving = false;
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public bool isRightClickPressed = false;
    public JobAnimations animationManager; // 애니메이션 매니저 추가

    void Awake()
    {
        controls = new PlayerControls();
        animationManager = GetComponent<JobAnimations>(); //  자동으로 연결
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

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            //  이동 속도 계산하여 애니메이션 실행

            animationManager.PlayMovementAnimation(moveSpeed);
            //Debug.Log("현재 Speed_Basic 값: " + speed);
            //  캐릭터 회전 처리
            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // 이동이 멈추면 Idle 애니메이션 실행
            if (!isRightClickPressed && Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
                animationManager.StopMovementAnimation();
            }
        }
    }

    public void OnRightClickStart(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isRightClickPressed = true;
            UpdateTargetPosition();
            isMoving = true;
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
            targetPosition = hit.point;
            isMoving = true;
        }
    }
}
