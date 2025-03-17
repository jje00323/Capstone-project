using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkill : MonoBehaviour
{

    private PlayerControls controls;
    private PlayerController playerController;
    private Camera mainCamera; //  카메라 참조 추가

    private bool isDashing = false;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.DashSkill.performed += ctx => TryDash();
        playerController = GetComponent<PlayerController>();

        mainCamera = Camera.main;
    }

    private void OnEnable() { controls.Player.Enable(); }
    private void OnDisable() { controls.Player.Disable(); }

    private void TryDash()
    {
        DashSettings dashSettings = JobManager.Instance.GetDashSettings();

        if (dashSettings.IsReady())
        {
            dashSettings.UseDash(); //  대쉬 사용 시간 기록
            StartCoroutine(Dash(dashSettings));
        }
        else
        {
            Debug.Log($"대쉬 사용 불가! 남은 쿨타임: {dashSettings.GetRemainingCooldown():F1}초");
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
