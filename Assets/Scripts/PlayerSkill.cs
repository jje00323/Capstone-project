using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerSkill : MonoBehaviour
{

    private PlayerControls controls;
    private PlayerController playerController;
    private Camera mainCamera;

    private bool isDashing = false;

    public Animator Actionanimator;

    private bool bAttacking = false; // ���� ������ ����
    private bool bComboEnable = false; // �޺� �Է��� �������� ����
    private bool bComboExist = false; // ���� ������ ����Ǿ����� ����


    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.DashSkill.performed += ctx => TryDash();
        controls.Player.OnLeftClick.performed += ctx => UpdateAttacking();

        playerController = GetComponent<PlayerController>();
        Actionanimator = GetComponent<Animator>();

        mainCamera = Camera.main;
    }

    private void OnEnable() { controls.Player.Enable(); }
    private void OnDisable() { controls.Player.Disable(); }

    private void UpdateAttacking()
    {


        if (bAttacking)
        {
            if (bComboEnable) // �޺� ���� ������ ���� �߰� �Է��� ���
            {
                bComboExist = true;
                Debug.Log("�߰� ���� �Է� ����");
            }
            return;
        }

        StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {
        if (playerController != null)
        {
            playerController.StopMovement();
            playerController.StopAllMovementAnimations();
        }
        bAttacking = true;
        bComboExist = false;

        LookAtMouse();


        //  �ִϸ��̼� ���� (Base Layer �� AttackState)
        Actionanimator.SetTrigger("NextCombo");
        Debug.Log("���� ����");

        //  �ִϸ��̼��� 30% ����Ǹ� ���� �Է��� ���� �� �ֵ��� ����
        yield return new WaitUntil(() =>
            Actionanimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3f &&
            !Actionanimator.IsInTransition(0));

        bComboEnable = true; //  ���� ���� �Է� ����
        Debug.Log("�޺� �Է� ����");

        //  �ִϸ��̼��� 70% ����Ǹ� �߰� �Է��� �����Ͽ� ��� ����
        yield return new WaitUntil(() =>
            Actionanimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f &&
            !Actionanimator.IsInTransition(0));

        if (bComboExist) //  �߰� �Է��� �������� ��� ���� ���� ����
        {
            Debug.Log("�߰� ���� ����");
            bComboEnable = false;
            StartCoroutine(PerformAttack());
        }
        else
        {
            //  ������ ������ ������ Idle ���·� ����
            yield return new WaitUntil(() =>
                Actionanimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f &&
                !Actionanimator.IsInTransition(0));
            playerController.StartMovement();
            Actionanimator.SetTrigger("endCombo");


            EndAttack(); //  ������ ������ ������ ���� �̸� ���� ó��
        }
    }

    private void LookAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 lookDirection = hit.point - transform.position;
            lookDirection.y = 0; //  Y�� ȸ�� ���� (�ٴڸ� ���� ��)

            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = targetRotation; // ��� ȸ��
            }
        }
    }

    private void EndAttack()
    {
        bAttacking = false;
        bComboEnable = false;
        Debug.Log("���� ���� �� ��� �ٽ� ���� ����");
        
    }


    public void Hit()
    {
        Debug.Log(" Hit �̺�Ʈ �߻�! (���� ���� ó��)");
        // ���⿡�� ���� ���� ������ �����ϸ� �� (��: Raycast, Collider �˻� ��)
    }

    private void TryDash()
    {
        DashSettings dashSettings = JobManager.Instance.GetDashSettings();

        if (dashSettings.IsReady())
        {
            dashSettings.UseDash(); //  �뽬 ��� �ð� ���
            StartCoroutine(Dash(dashSettings));
        }
        else
        {
            Debug.Log($"�뽬 ��� �Ұ�! ���� ��Ÿ��: {dashSettings.GetRemainingCooldown():F1}��");
        }
    }
    private IEnumerator Dash(DashSettings dashSettings)
    {
        if (isDashing) yield break;
        isDashing = true;

        if (playerController != null)
        {
            playerController.StopMovement();
            playerController.StopAllMovementAnimations();
        }

        Vector3 dashDirection;
        if (dashSettings.dashTowardMouse)
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            dashDirection = (mouseWorldPosition - transform.position).normalized;
        }
        else
        {
            dashDirection = transform.forward;
        }

        if (dashSettings.dashTowardMouse && dashDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dashDirection);
        }

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + dashDirection * dashSettings.dashDistance;

        float elapsedTime = 0f;
        while (elapsedTime < dashSettings.dashDuration)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, (dashSettings.dashDistance / dashSettings.dashDuration) * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        isDashing = false;
        playerController.StartMovement();
    }












    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }

        return transform.position + transform.forward * 5f;
    }
}
