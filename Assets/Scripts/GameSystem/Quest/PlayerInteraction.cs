using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject interactUI;

    private NPCInteraction currentNPC;

    private PlayerControls inputActions;

    private void Awake()
    {
        inputActions = new PlayerControls();

        // G 키에 해당하는 Talk_NPC 액션 연결
        inputActions.UI.Talk_NPC.performed += ctx =>
        {
            Debug.Log("[Input] G 키(Talk_NPC) 입력 감지됨");

            if (currentNPC != null)
            {
                Debug.Log($"[Input] 대화 시작 시도 → 대상: {currentNPC.name}");
                currentNPC.Interact();
            }
            else
            {
                Debug.LogWarning("[Input] 현재 상호작용 가능한 NPC 없음");
            }
        };
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.UI.Enable(); //  이것이 없으면 Talk_NPC 작동 안 함
    }
    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.UI.Disable();
    }

    private void Start()
    {
        if (interactUI != null)
            interactUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[TriggerEnter] 충돌한 오브젝트: {other.name}");

        if (other.TryGetComponent(out NPCInteraction npc))
        {
            currentNPC = npc;
            Debug.Log($"[PlayerInteraction] NPC 감지됨: {npc.name}");

            if (interactUI != null)
                interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out NPCInteraction npc) && npc == currentNPC)
        {
            currentNPC = null;

            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }
}