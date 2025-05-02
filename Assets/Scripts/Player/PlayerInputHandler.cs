using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerInputHandler : MonoBehaviour
{
    private PlayerControls controls;
    private PlayerStateMachine stateMachine;
    private PlayerMovement movement;
    private PlayerAttack attack;
    private PlayerDodge dodge;
    private MoveController moveController;
    private PlayerSkillController skillController;

    private bool blockRightClickThisFrame = false;

    void Awake()
    {
        controls = new PlayerControls();
        stateMachine = GetComponent<PlayerStateMachine>();
        movement = GetComponent<PlayerMovement>();
        attack = GetComponent<PlayerAttack>();
        dodge = GetComponent<PlayerDodge>();
        moveController = GetComponent<MoveController>();
        skillController = GetComponent<PlayerSkillController>();

        controls.Player.OnLeftClick.performed += ctx =>
        {
            attack.HandleAttackInput();
            BlockRightClickForOneFrame();
        };

        controls.Player.DashSkill.performed += ctx =>
        {
            dodge.HandleDodge();
            BlockRightClickForOneFrame();
        };

        controls.Player.OnRightClick.performed += ctx =>
        {
            if (!blockRightClickThisFrame)
                HandleRightClick();
        };

    }

    void Update()
    {
        blockRightClickThisFrame = false;
    }

    private void HandleRightClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 destination = hit.point;
            moveController?.HandleRightClickInput(destination, true); // 마커 있는 최초 클릭만
        }
    }

    public void BlockRightClickForOneFrame()
    {
        blockRightClickThisFrame = true;
    }

    public void AllowRightClick()
    {
        blockRightClickThisFrame = false;
    }

    public bool IsRightClickAllowed()
    {
        return !blockRightClickThisFrame;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();
}