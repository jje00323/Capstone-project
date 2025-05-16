using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class QuestSlotUI : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI progressText;

    [SerializeField] public QuestDetailUI detailUI;

    private PlayerQuestProgress questProgress;

    public void Setup(PlayerQuestProgress progress)
    {
        questProgress = progress;
        QuestData data = progress.questData;

        // 퀘스트 타입 표시
        typeText.text = data.questType switch
        {
            QuestType.Main => "<color=#FFA500>메인</color>",     // 주황색
            QuestType.Side => "<color=#00BFFF>서브</color>",     // 하늘색
            QuestType.Repeatable => "<color=#AAAAAA>반복</color>",
            QuestType.Event => "<color=#FFD700>이벤트</color>",
            _ => "기타"
        };

        // 제목
        titleText.text = data.questTitle;

        // 조건 진행도 요약
        int total = progress.conditionProgress.Count;
        int complete = 0;
        for (int i = 0; i < total; i++)
        {
            if (data.conditions[i].IsMet(progress))
                complete++;
        }

        progressText.text = $"<color=green>({complete} / {total})</color>";

        // 클릭 시 상세 정보 보기
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() =>
        {
            detailUI.ShowDetails(progress);
        });
    }
}