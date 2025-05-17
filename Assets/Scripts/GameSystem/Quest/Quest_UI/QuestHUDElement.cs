using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestHUDElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI conditionText;
    [SerializeField] private Button completeButton;

    public void Setup(PlayerQuestProgress progress)
    {
        QuestData data = progress.questData;
        titleText.text = data.questTitle;

        int total = data.conditions.Count;
        int complete = 0;

        System.Text.StringBuilder sb = new();
        for (int i = 0; i < total; i++)
        {
            string cond = data.conditions[i].conditionName;
            int cur = progress.conditionProgress[i];

            string detail = data.conditions[i] switch
            {
                KillEnemyCondition k => $"{cur}/{k.requiredKillCount}",
                CollectItemCondition c => $"{cur}/{c.requiredAmount}",
                TalkToNPCCondition => cur >= 1 ? "완료" : "미완료",
                ReachAreaCondition => cur >= 1 ? "완료" : "미완료",
                UseItemCondition u => $"{cur}/{u.requiredUseCount}",
                _ => $"{cur} / ?"
            };
            sb.AppendLine($"- {cond}: {detail}");

            if (data.conditions[i].IsMet(progress))
                complete++;
        }

        conditionText.text = sb.ToString();
        completeButton.gameObject.SetActive(progress.state == QuestState.Completed);
        completeButton.onClick.RemoveAllListeners();
        completeButton.onClick.AddListener(() =>
        {
            QuestManager.Instance.CompleteQuest(data);
            gameObject.SetActive(false);
        });
    }
}