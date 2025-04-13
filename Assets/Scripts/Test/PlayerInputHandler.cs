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

    void Awake()
    {
        controls = new PlayerControls();
        stateMachine = GetComponent<PlayerStateMachine>();
        movement = GetComponent<PlayerMovement>();
        attack = GetComponent<PlayerAttack>();
        dodge = GetComponent<PlayerDodge>();

        //controls.Player.OnRightClick.performed += ctx => movement.HandleRightClick();
        controls.Player.OnLeftClick.performed += ctx => attack.HandleAttackInput();
        controls.Player.DashSkill.performed += ctx => dodge.HandleDodge();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();
}
