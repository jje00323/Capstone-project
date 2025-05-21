using System.Collections;
using UnityEngine;

public class BossSummonTrigger : MonoBehaviour
{
    [Header("º¸½º ½ºÆ÷³Ê ÇÁ¸®ÆÕ")]
    public GameObject bossSpawnerPrefab;

    [Header("ÄÆ¾À ¹× ÆäÀÌµå ÇÁ¸®ÆÕ")]
    public GameObject cutsceneTrackPrefab;

    [Header("ÀÎÆ®·Î Ä«¸Þ¶ó ÇÁ¸®ÆÕ")]
    public GameObject introTrackPrefab;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;
        StartCoroutine(SummonBossSequence());
    }

    private IEnumerator SummonBossSequence()
    {
        yield return new WaitForSeconds(1f);

        if (introTrackPrefab != null)
            Instantiate(introTrackPrefab, transform.position, Quaternion.identity);

        if (cutsceneTrackPrefab != null)
            Instantiate(cutsceneTrackPrefab, transform.position, Quaternion.identity);

        if (bossSpawnerPrefab != null)
        {
            var spawnPos = transform.position + Vector3.up * 12f;
            var spawnerGO = Instantiate(bossSpawnerPrefab, spawnPos, transform.rotation);
            var spawner = spawnerGO.GetComponent<EnemySpawner>();

            if (spawner != null)
            {
                spawner.SpawnAll();
                yield return null;

                var boss = spawner.spawnedBoss;
                if (boss != null)
                {
                    var rb = boss.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.isKinematic = false;
                        rb.useGravity = true;
                    }

                    yield return new WaitUntil(() => GameObject.Find("BossIntroTrack(Clone)") != null);

                    var fsm = boss.GetComponent<BossEnemyFSM>();
                    if (fsm != null)
                        fsm.ChangeState(fsm.introState);
                }
            }
        }

        Destroy(gameObject);
    }
}