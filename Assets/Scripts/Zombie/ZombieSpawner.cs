using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public GameObject zombiePrefab;      
    public ZombieSO zombieData;          
    public float spawnRadius = 10f;      
    public float minSpawnInterval = 3f;  
    public float maxSpawnInterval = 7f;  

    private float nextSpawnTime;

    private void Start()
    {
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        int segments = 32;
        Vector3 prev = transform.position + new Vector3(spawnRadius, 0, 0);
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 newPos = transform.position + new Vector3(Mathf.Cos(angle) * spawnRadius, 0, Mathf.Sin(angle) * spawnRadius);
            Gizmos.DrawLine(prev, newPos);
            prev = newPos;
        }
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnZombie();
            nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    private void SpawnZombie()
    {
        if (zombiePrefab == null || zombieData == null) return;

        Vector3 spawnPos = transform.position + Random.insideUnitSphere * spawnRadius;
        spawnPos.y = transform.position.y;

        GameObject zombieObj = Instantiate(zombiePrefab, spawnPos, Quaternion.identity);

        Zombie zombie = zombieObj.GetComponent<Zombie>();
        if (zombie != null)
        {
            zombie.Initialize(zombieData);
        }

        if (zombieData.isBoss)
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowBossSpawnMessage();
            }
            else
            {
                Debug.LogWarning("UIManager.Instance가 존재하지 않아 메시지를 표시할 수 없습니다.");
            }
        }
    }
}