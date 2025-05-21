using System.Collections;
using UnityEngine;

public class BossSummonTrigger : MonoBehaviour
{
    [Header("보스 스포너 프리팹")]
    public GameObject bossSpawnerPrefab;

    [Header("컷씬 및 페이드 프리팹")]
    public GameObject cutsceneTrackPrefab;

    [Header("인트로 카메라 프리팹")]
    public GameObject introTrackPrefab;

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
            var bossObj = Instantiate(bossSpawnerPrefab, transform.position, transform.rotation);
            Debug.Log("[BossSummonTrigger] 보스 스포너 생성됨");

            // FSM 상태 진입은 보스 스크립트 내부에서 처리하거나 여기서도 가능
            var fsm = bossObj.GetComponentInChildren<BossEnemyFSM>();
            if (fsm != null)
                fsm.ChangeState(fsm.introState);
        }

        if (introTrackPrefab != null)
            Instantiate(introTrackPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject); // 마법진 제거
    }
}

