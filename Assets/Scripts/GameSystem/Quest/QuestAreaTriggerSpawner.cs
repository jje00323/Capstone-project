using System.Collections.Generic;
using UnityEngine;

public class QuestAreaTriggerSpawner : MonoBehaviour
{
    public static QuestAreaTriggerSpawner Instance { get; private set; }

    [Header("트리거 프리팹")]
    [SerializeField] private GameObject areaTriggerPrefab;

    [Header("지역 위치 매핑")]
    [SerializeField] private List<AreaMapping> areaMappings = new List<AreaMapping>();

    private Dictionary<string, Vector3> areaPositions = new Dictionary<string, Vector3>();
    private Dictionary<string, GameObject> spawnedTriggers = new Dictionary<string, GameObject>(); //  추가

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (var mapping in areaMappings)
        {
            if (!areaPositions.ContainsKey(mapping.areaID))
            {
                areaPositions.Add(mapping.areaID, mapping.position);
            }
        }
    }

    /// <summary>
    /// 지정된 지역 ID에 대한 트리거를 생성
    /// </summary>
    public void SpawnTrigger(string areaID)
    {
        if (spawnedTriggers.ContainsKey(areaID))
        {
            Debug.Log($"[QuestAreaTriggerSpawner] 이미 생성됨: {areaID}");
            return;
        }

        if (!areaPositions.TryGetValue(areaID, out var spawnPos))
        {
            Debug.LogWarning($"[QuestAreaTriggerSpawner] 위치 정보 없음: {areaID}");
            return;
        }

        if (areaTriggerPrefab == null)
        {
            Debug.LogError("[QuestAreaTriggerSpawner] areaTriggerPrefab이 비어 있음.");
            return;
        }

        GameObject obj = Instantiate(areaTriggerPrefab, spawnPos, Quaternion.identity);
        obj.name = $"AreaTrigger_{areaID}";

        var trigger = obj.GetComponent<QuestAreaTrigger>();
        if (trigger != null)
        {
            trigger.Setup(areaID);
        }

        spawnedTriggers.Add(areaID, obj); //  등록
        Debug.Log($"[QuestAreaTriggerSpawner] 트리거 생성 완료: {areaID}");
    }

    /// <summary>
    /// 트리거 제거
    /// </summary>
    public void RemoveTrigger(string areaID)
    {
        if (spawnedTriggers.TryGetValue(areaID, out var obj))
        {
            Destroy(obj);
            spawnedTriggers.Remove(areaID);
            Debug.Log($"[QuestAreaTriggerSpawner] 트리거 제거됨: {areaID}");
        }
        else
        {
            Debug.LogWarning($"[QuestAreaTriggerSpawner] 제거 대상 없음: {areaID}");
        }
    }

    [System.Serializable]
    public class AreaMapping
    {
        public string areaID;
        public Vector3 position;
    }
}