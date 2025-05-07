using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("적 프리팹 및 스폰 위치")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;


    private void Start()
    {
        SpawnAll();
    }
    public void SpawnAll()
    {
        foreach (Transform spawn in spawnPoints)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawn.position, spawn.rotation);
            SetupNavMeshObstacle(enemy);
        }
    }

    private void SetupNavMeshObstacle(GameObject enemy)
    {
        // 보스는 NavMeshObstacle을 직접 프리팹에서 할당하므로 제외
        if (enemy.CompareTag("Boss")) return;

        var obstacle = enemy.GetComponent<NavMeshObstacle>();
        if (obstacle == null)
        {
            obstacle = enemy.AddComponent<NavMeshObstacle>();
        }

        obstacle.carving = true;
        obstacle.carveOnlyStationary = false;
        obstacle.shape = NavMeshObstacleShape.Capsule;
        obstacle.radius = 0.3f;
        obstacle.height = 2.0f;
    }
}
