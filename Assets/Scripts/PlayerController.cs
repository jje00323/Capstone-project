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
    public JobAnimations animationManager; // �ִϸ��̼� �Ŵ��� �߰�

    public Animator animator;

    void Awake()
    {
        controls = new PlayerControls();
        animationManager = GetComponent<JobAnimations>(); //  �ڵ����� ����
        animator = GetComponent<Animator>();
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

            //  �̵� �ӵ� ����Ͽ� �ִϸ��̼� ����
            
            animationManager.PlayMovementAnimation(moveSpeed);
            //Debug.Log("���� Speed_Basic ��: " + speed);
            //  ĳ���� ȸ�� ó��
            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // �̵��� ���߸� Idle �ִϸ��̼� ����
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

    public void StopMovement()
    {
        isMoving = false;
        Debug.Log("PlayerController: �̵� ����");

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
            Debug.Log("��� �ִϸ��̼� �Ķ���� 0���� ����");
        }
    }
}
