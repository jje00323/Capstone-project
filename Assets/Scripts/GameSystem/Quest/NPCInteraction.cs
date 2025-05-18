using System.Collections;
using UnityEngine;
using TMPro;

public class NPCInteraction : MonoBehaviour
{
    [Header("NPC 설정")]
    [SerializeField] private string npcID = "TUTORIAL_NPC";
    [TextArea][SerializeField] private string[] dialogueLines;
    [SerializeField] private string npcName = "???";  // Inspector에서 설정
    [SerializeField] private TextMeshProUGUI npcNameText;

    [Header("UI 연결")]
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private TextMeshProUGUI dialogueText;

    private int currentLine = 0;
    private bool isTalking = false;
    private bool questConditionUpdated = false;

    private void Start()
    {
        if (dialogueUI != null)
            dialogueUI.SetActive(false);
    }

    // 상호작용 (예: 버튼, 범위 내 키 입력)
    public void Interact()
    {
        Debug.Log($"[NPCInteraction] Interact() 호출됨 → npcID: {npcID}");

        if (isTalking) return;

        isTalking = true;
        currentLine = 0;
        questConditionUpdated = false;

        if (npcNameText != null)
            npcNameText.text = npcName;

        dialogueUI.SetActive(true);
        ShowNextLine();
    }

    private void Update()
    {
        if (isTalking && Input.GetKeyDown(KeyCode.F))
        {
            ShowNextLine();
        }
    }

    private void ShowNextLine()
    {
        Debug.Log($"[NPCInteraction] 대화 {currentLine + 1}번째 줄 표시 시도");

        if (currentLine < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentLine];
            currentLine++;

            if (!questConditionUpdated)
            {
                Debug.Log($"[NPCInteraction] 퀘스트 조건 갱신: {npcID}");
                QuestManager.Instance.UpdateCondition("TalkToNPC", npcID);
                questConditionUpdated = true;
            }
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        isTalking = false;
        dialogueUI.SetActive(false);
        dialogueText.text = "";
    }
}