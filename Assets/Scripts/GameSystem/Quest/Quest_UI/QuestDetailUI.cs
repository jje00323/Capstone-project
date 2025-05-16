using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;

public class QuestDetailUI : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI conditionText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Button completeButton;

    [Header("보상 슬롯")]
    [SerializeField] private RewardSlotUI[] rewardItemSlots;  // 슬롯을 미리 배치해서 배열로 연결

    private PlayerQuestProgress currentProgress;

    private void OnEnable()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestUpdated += OnQuestProgressUpdated;
    }

    private void OnDisable()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestUpdated -= OnQuestProgressUpdated;
    }

    private void OnQuestProgressUpdated(PlayerQuestProgress updated)
    {
        // 현재 보고 있는 퀘스트가 갱신되었으면 다시 표시
        if (currentProgress != null && updated.questData.questID == currentProgress.questData.questID)
        {
            ShowDetails(updated);
        }
    }
    public void ShowDetails(PlayerQuestProgress progress)
    {
        currentProgress = progress;
        var data = progress.questData;

        titleText.text = data.questTitle;

        // 진행도 계산
        int total = progress.conditionProgress.Count;
        int complete = 0;
        for (int i = 0; i < total; i++)
        {
            if (data.conditions[i].IsMet(progress))
                complete++;
        }

        objectiveText.text = $"{data.questObjective} (<color=green>{complete} / {total}</color>)";
        descriptionText.text = data.questDescription;

        // 조건 구성
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < data.conditions.Count; i++)
        {
            var condition = data.conditions[i];
            int cur = progress.conditionProgress[i];
            string condName = condition.conditionName;
            string detail = condition switch
            {
                KillEnemyCondition k => $"{cur}/{k.requiredKillCount}",
                CollectItemCondition c => $"{cur}/{c.requiredAmount}",
                TalkToNPCCondition => cur >= 1 ? "완료" : "미완료",
                ReachAreaCondition => cur >= 1 ? "완료" : "미완료",
                UseItemCondition u => $"{cur}/{u.requiredUseCount}",
                _ => $"{cur} / ?"
            };
            sb.AppendLine($"- {condName}: {detail}");
        }
        conditionText.text = sb.ToString();

        // EXP, Gold 텍스트
        rewardText.text = $"EXP: {data.reward.experience}\nGold: {data.reward.gold}";

        // 슬롯 초기화 및 보상 삽입
        for (int i = 0; i < rewardItemSlots.Length; i++)
        {
            if (i < data.reward.items.Count)
            {
                rewardItemSlots[i].Setup(data.reward.items[i]);
            }
            else
            {
                rewardItemSlots[i].Clear();
            }
        }

        // 완료 버튼
        completeButton.interactable = progress.state == QuestState.Completed;
        completeButton.onClick.RemoveAllListeners();
        completeButton.onClick.AddListener(() =>
        {
            QuestManager.Instance.CompleteQuest(data);
            completeButton.interactable = false;
            conditionText.text += "\n→ 완료됨!";
        });

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void RefreshCurrent()
    {
        if (currentProgress != null)
        {
            ShowDetails(currentProgress); // 강제로 다시 갱신
        }
    }
}