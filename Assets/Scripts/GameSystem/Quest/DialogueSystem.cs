using TMPro;
using UnityEngine;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance;

    [SerializeField] private GameObject narrationPanel;
    [SerializeField] private TextMeshProUGUI narrationText;

    private bool isNarrationActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("[DialogueSystem] 싱글톤 인스턴스 등록 성공");
        }
        else
        {
            Debug.LogWarning("[DialogueSystem] 중복 인스턴스 발견, 파괴됨");
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (isNarrationActive && Input.GetKeyDown(KeyCode.F))
        {
            CloseNarration();
        }
    }

    public void ShowNarration(string text)
    {
        if (narrationPanel == null)
        {
            Debug.LogError("[DialogueSystem] narrationPanel이 연결되지 않았습니다.");
            return;
        }
        if (narrationText == null)
        {
            Debug.LogError("[DialogueSystem] narrationText가 연결되지 않았습니다.");
            return;
        }

        narrationText.text = text;
        narrationPanel.SetActive(true);
        isNarrationActive = true;
        Debug.Log("[DialogueSystem] 내레이션 시작됨");
    }

    public void CloseNarration()
    {
        narrationPanel.SetActive(false);
        isNarrationActive = false;
        Debug.Log("[DialogueSystem] 내레이션 닫힘 (F 키)");
    }
}