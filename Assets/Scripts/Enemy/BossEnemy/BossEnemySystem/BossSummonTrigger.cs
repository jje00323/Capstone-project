using System.Collections;
using UnityEngine;

public class BossSummonTrigger : MonoBehaviour
{
    [Header("보스 스포너 프리팹")]
    public GameObject bossSpawnerPrefab;

    [Header("컷씬 및 페이드 프리팹")]
    public GameObject cutsceneTrackPrefab;
    public GameObject fadeCanvasPrefab;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        triggered = true; // 첫 줄에서 바로 true 설정

        if (!other.CompareTag("Player"))
        {
            Debug.Log("[BossSummonTrigger] 태그 불일치 - 무시");
            return;
        }

        Debug.Log("[BossSummonTrigger] 플레이어 진입 감지됨 → 보스 소환 시작");
        StartCoroutine(SummonBossSequence());
    }

    private IEnumerator SummonBossSequence()
    {
        yield return new WaitForSeconds(1f);

        if (bossSpawnerPrefab != null)
        {
            Instantiate(bossSpawnerPrefab, transform.position, transform.rotation);
            Debug.Log("[BossSummonTrigger] 보스 스포너 생성됨 (SpawnAll은 자동 호출됨)");
        }

        if (cutsceneTrackPrefab != null)
            Instantiate(cutsceneTrackPrefab, transform.position, transform.rotation);

        if (fadeCanvasPrefab != null)
            Instantiate(fadeCanvasPrefab);

        Destroy(gameObject); // 마법진 제거
    }
}

