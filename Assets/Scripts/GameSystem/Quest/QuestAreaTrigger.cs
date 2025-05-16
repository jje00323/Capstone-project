using UnityEngine;

public class QuestAreaTrigger : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private string areaID;
    [SerializeField] private GameObject visualEffect;

    private bool hasTriggered = false;

    public void Setup(string id)
    {
        areaID = id;

        // 이펙트는 항상 활성화해도 됨 (이미 조건에 의해 생성됨)
        if (visualEffect != null)
            visualEffect.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag("Player")) return;

        hasTriggered = true;

        // 조건 갱신 시도
        QuestManager.Instance.UpdateCondition("ReachArea", areaID);

        // 트리거 직접 제거
        QuestAreaTriggerSpawner.Instance?.RemoveTrigger(areaID);

        if (visualEffect != null)
            visualEffect.SetActive(false);
    }
}