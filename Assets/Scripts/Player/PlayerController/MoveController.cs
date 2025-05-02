using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerStateMachine))]
[RequireComponent(typeof(PlayerInputHandler))]
public class MoveController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerStateMachine stateMachine;
    private PlayerInputHandler inputHandler;

    [SerializeField] private GameObject moveClickEffectPrefab;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        stateMachine = GetComponent<PlayerStateMachine>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    public void HandleRightClickInput(Vector3 destination)
    {
        if (!inputHandler.IsRightClickAllowed()) return;

        if (NavMesh.SamplePosition(destination, out NavMeshHit navHit, 2.0f, NavMesh.AllAreas))
        {
            Vector3 targetPosition = navHit.position;

            if (stateMachine.CanMove())
            {
                movement.MoveTo(targetPosition, 0.5f, null);
                SpawnMoveEffect(targetPosition);
            }
            else
            {
                movement.SetPendingMove(targetPosition);
            }
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

    //애니메이션 이벤트로 호출될 수 있는 메서드 예시
    public void EndOfInput()
    {
        inputHandler.AllowRightClick();
    }
}