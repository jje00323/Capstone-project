using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerStateMachine))]
[RequireComponent(typeof(PlayerInputHandler))]
public class MoveController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerStateMachine stateMachine;
    private PlayerInputHandler inputHandler;

    [SerializeField] private GameObject moveClickEffectPrefab;

    private bool isRightClickHeld = false;
    private float rightClickTimer = 0f;
    private float rightClickRepeatThreshold = 0.15f;
    private bool hasTriggeredHoldAction = false;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        stateMachine = GetComponent<PlayerStateMachine>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            isRightClickHeld = true;
            rightClickTimer = 0f;
            hasTriggeredHoldAction = false;
            TryHandleRightClick(true); // 클릭 1회만 마커 생성 허용
        }
        else if (Mouse.current.rightButton.isPressed && isRightClickHeld)
        {
            rightClickTimer += Time.deltaTime;
            if (!hasTriggeredHoldAction && rightClickTimer >= rightClickRepeatThreshold)
            {
                TryHandleRightClick(false); // 꾹 누름은 마커 생성 없이 이동만
                hasTriggeredHoldAction = true;
            }
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            isRightClickHeld = false;
            rightClickTimer = 0f;
            hasTriggeredHoldAction = false;
        }
    }

    public void HandleRightClickInput(Vector3 destination, bool spawnEffect)
    {
        if (!inputHandler.IsRightClickAllowed()) return;

        if (NavMesh.SamplePosition(destination, out NavMeshHit navHit, 2.0f, NavMesh.AllAreas))
        {
            Vector3 targetPosition = navHit.position;

            if (stateMachine.CanMove())
            {
                movement.MoveTo(targetPosition, 0.5f, null);
                if (spawnEffect)
                    SpawnMoveEffect(targetPosition);
            }
            else
            {
                movement.SetPendingMove(targetPosition);
            }
        }
    }

    private void TryHandleRightClick(bool spawnEffect)
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 destination = hit.point;
            HandleRightClickInput(destination, spawnEffect);
        }
    }

    private void SpawnMoveEffect(Vector3 position)
    {
        if (moveClickEffectPrefab != null)
        {
            GameObject fx = Instantiate(moveClickEffectPrefab, position + Vector3.up * 0.1f, Quaternion.identity);
            fx.transform.localScale = Vector3.one * 0.7f;
            Destroy(fx, 2f);
        }
    }

    public void EndOfInput()
    {
        inputHandler.AllowRightClick();
    }
}
