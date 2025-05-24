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
            Debug.Log("싱글톤 인스턴스 등록 성공");

            // 자동 연결 시도
            if (narrationPanel == null)
            {
                narrationPanel = GameObject.Find("System_text");
                if (narrationPanel == null)
                    Debug.LogError("'System_text 오브젝트를 찾을 수 없습니다.");
            }

            if (narrationText == null && narrationPanel != null)
            {
                narrationText = narrationPanel.GetComponentInChildren<TextMeshProUGUI>();
                if (narrationText == null)
                    Debug.LogError("narrationText(TMP) 컴포넌트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("중복 인스턴스 발견, 파괴됨");
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
        if (narrationPanel == null || narrationText == null)
        {
            Debug.LogError(" UI 컴포넌트가 연결되지 않았습니다.");
            return;
        }

        narrationText.text = text;
        narrationPanel.SetActive(true);
        isNarrationActive = true;
    }

    public void CloseNarration()
    {
        if (narrationPanel != null)
            narrationPanel.SetActive(false);

        isNarrationActive = false;
    }
}