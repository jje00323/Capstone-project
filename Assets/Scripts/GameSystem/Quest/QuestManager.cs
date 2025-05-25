using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    private List<PlayerQuestProgress> activeQuests = new List<PlayerQuestProgress>();

    public event System.Action<PlayerQuestProgress> OnQuestUpdated;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 퀘스트 수락
    public void AcceptQuest(QuestData questData)
    {
        if (IsQuestActive(questData.questID)) return;

        PlayerQuestProgress newProgress = new PlayerQuestProgress(questData);
        activeQuests.Add(newProgress);

        foreach (var condition in questData.conditions)
        {
            if (condition is ReachAreaCondition areaCond)
            {
                QuestAreaTriggerSpawner.Instance?.SpawnTrigger(areaCond.areaID);
            }
        }

        if (!string.IsNullOrWhiteSpace(questData.startNarration))
        {
            Debug.Log($"[QuestManager] 내레이션 출력 시도: {questData.startNarration}");
            DialogueSystem.Instance?.ShowNarration(questData.startNarration);
        }

        OnQuestUpdated?.Invoke(newProgress);
        Debug.Log($"[QuestManager] 퀘스트 수락: {questData.questTitle}");
    }

    // 조건 충족 시 진척도 증가
    public void UpdateCondition(string type, string identifier, int amount = 1)
    {
        foreach (var progress in activeQuests)
        {
            if (progress.state != QuestState.InProgress) continue;

            for (int i = 0; i < progress.questData.conditions.Count; i++)
            {
                var condition = progress.questData.conditions[i];

                if (condition is KillEnemyCondition killCond && type == "KillEnemy" && killCond.targetEnemyTag == identifier)
                {
                    progress.IncrementProgress(i, amount);
                }
                else if (condition is CollectItemCondition collectCond && type == "CollectItem" && collectCond.targetItem.itemName == identifier)
                {
                    progress.IncrementProgress(i, amount);
                }
                else if (condition is TalkToNPCCondition talkCond && type == "TalkToNPC" && talkCond.npcID == identifier)
                {
                    progress.IncrementProgress(i, amount);
                }
                else if (condition is ReachAreaCondition areaCond && type == "ReachArea" && areaCond.areaID == identifier)
                {
                    progress.IncrementProgress(i, amount);
                    QuestAreaTriggerSpawner.Instance?.RemoveTrigger(identifier);
                }
                else if (condition is UseItemCondition useCond && type == "UseItem" && useCond.requiredItem.itemName == identifier)
                {
                    progress.IncrementProgress(i, amount);
                }
            }

            if (progress.IsAllConditionsMet() && progress.state != QuestState.Completed)
            {
                progress.state = QuestState.Completed;
                Debug.Log($"[QuestManager] 퀘스트 완료 가능: {progress.questData.questTitle}");
            }

            OnQuestUpdated?.Invoke(progress);
        }
    }

    // 퀘스트 보상 수령 처리
    public void CompleteQuest(QuestData questData)
    {
        PlayerQuestProgress progress = GetProgress(questData.questID);
        if (progress == null || progress.state != QuestState.Completed) return;

        GrantRewards(questData.reward);
        progress.state = QuestState.Rewarded;

        if (questData.nextQuestID != -1)
        {
            string prefix = questData.resourcePrefix ?? "Quest_";
            string path = $"Quest/{prefix}{questData.nextQuestID}";

            QuestData nextQuest = Resources.Load<QuestData>(path);
            if (nextQuest != null)
            {
                AcceptQuest(nextQuest);
                Debug.Log($"[QuestManager] 연계 퀘스트 로드됨: {nextQuest.questTitle}");
            }
            else
            {
                Debug.LogWarning($"[QuestManager] 연계 퀘스트를 찾을 수 없음: {path}");
            }
        }

        Debug.Log($"[QuestManager] 보상 수령 완료: {questData.questTitle}");
    }

    private void GrantRewards(QuestReward reward)
    {
        var player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[QuestManager] 플레이어 객체를 찾을 수 없습니다.");
            return;
        }

        var status = player.GetComponent<PlayerStatus>();
        if (status != null)
        {
            status.GainEXP(reward.experience);
        }
        else
        {
            Debug.LogWarning("[QuestManager] PlayerStatus 컴포넌트를 찾을 수 없습니다.");
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddGold(reward.gold);
            foreach (var item in reward.items)
            {
                InventoryManager.Instance.AddItem(item.item, item.amount);
            }
        }
        else
        {
            Debug.LogWarning("[QuestManager] InventoryManager.Instance가 존재하지 않습니다.");
        }
    }

    public PlayerQuestProgress GetProgress(int questID)
    {
        return activeQuests.Find(q => q.questData.questID == questID);
    }

    public bool IsQuestActive(int questID)
    {
        return activeQuests.Exists(q => q.questData.questID == questID);
    }

    public List<PlayerQuestProgress> GetActiveQuests()
    {
        return activeQuests;
    }

    public void ResetAllQuests()
    {
        activeQuests.Clear();
        Debug.Log("[QuestManager] 모든 퀘스트 초기화 완료");
    }
}