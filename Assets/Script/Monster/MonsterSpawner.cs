using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MonsterSpawner : MonoBehaviour
{
    public static MonsterSpawner Instance;

    [Header("���� ����")]
    public List<MonsterSpawnData> monsterTypes;
    public int maxMonsters = 10;
    public float spawnInterval = 5f;

    [Header("���� ����")]
    public Vector2 spawnAreaMin = new Vector2(-20, -20);
    public Vector2 spawnAreaMax = new Vector2(20, 20);
    public float minDistanceFromPlayer = 10f;

    private List<Monster> activeMonsters = new List<Monster>();
    private Dictionary<string, Queue<Monster>> monsterPools = new Dictionary<string, Queue<Monster>>();
    private float spawnTimer;
    private float cachedTotalWeight;
    private Transform player;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        InitializePool();
        CacheTotalWeight();

        for (int i = 0; i < maxMonsters / 2; i++)
        {
            SpawnMonster();
        }
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval && activeMonsters.Count < maxMonsters)
        {
            SpawnMonster();
            spawnTimer = 0f;
        }
    }

    void CacheTotalWeight()
    {
        cachedTotalWeight = 0f;
        foreach (var data in monsterTypes)
            cachedTotalWeight += data.spawnWeight;
    }

    void InitializePool()
    {
        foreach (var spawnData in monsterTypes)
        {
            string monsterName = spawnData.monsterPrefab.monsterName;

            if (!monsterPools.ContainsKey(monsterName))
            {
                monsterPools[monsterName] = new Queue<Monster>();
            }

            for (int i = 0; i < spawnData.poolSize; i++)
            {
                Monster monster = Instantiate(spawnData.monsterPrefab, transform);
                monster.spawner = this;
                monster.gameObject.tag = "Enemy";
                monster.gameObject.layer = LayerMask.NameToLayer("Enemy");
                monster.gameObject.SetActive(false);

                monsterPools[monsterName].Enqueue(monster);
            }
        }
    }

    public void SpawnMonster()
    {
        if (monsterTypes.Count == 0) return;

        float randomValue = Random.Range(0f, cachedTotalWeight);
        float currentWeight = 0f;

        MonsterSpawnData selectedData = monsterTypes[0];

        foreach (var data in monsterTypes)
        {
            currentWeight += data.spawnWeight;
            if (randomValue <= currentWeight)
            {
                selectedData = data;
                break;
            }
        }

        Monster monster = GetMonsterFromPool(selectedData.monsterPrefab);

        if (monster != null)
        {
            Vector2 spawnPos = GetRandomSpawnPosition();
            monster.transform.position = spawnPos;
            monster.gameObject.SetActive(true);
            monster.OnSpawn();

            activeMonsters.Add(monster);

        }
        else
        {
            Debug.LogWarning("No available monsters in pool!");
        }
    }

    Monster GetMonsterFromPool(Monster prefab)
    {
        string monsterName = prefab.monsterName;

        if (monsterPools.ContainsKey(monsterName) && monsterPools[monsterName].Count > 0)
        {
            Monster monster = monsterPools[monsterName].Dequeue();
            return monster;
        }
        return null; // �� Ǯ ��� null ��ȯ (���� ���� �� ��!)
    }

    public void ReturnMonsterToPool(Monster monster)
    {
        if (monster == null) return;

        string monsterName = monster.monsterName;

        activeMonsters.Remove(monster);
        monster.gameObject.SetActive(false);

        if (monsterPools.ContainsKey(monsterName))
        {
            monsterPools[monsterName].Enqueue(monster);
        }
    }

    Vector2 GetRandomSpawnPosition()
    {
        Vector2 spawnPos;
        int attempts = 0;

        do
        {
            // ������ ��ġ �������� ��� ��ǥ ���
            float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x) + transform.position.x;
            float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y) + transform.position.y;
            spawnPos = new Vector2(x, y);
            attempts++;

        } while (player != null &&
                 Vector2.Distance(spawnPos, player.position) < minDistanceFromPlayer &&
                 attempts < 20);

        return spawnPos;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        // ������ ��ġ �������� �ڽ� �׸���
        Vector2 center = (spawnAreaMin + spawnAreaMax) / 2f + (Vector2)transform.position;
        Vector2 size = spawnAreaMax - spawnAreaMin;

        Gizmos.DrawWireCube(center, size);
    }
}

[System.Serializable]
public class MonsterSpawnData
{
    public Monster monsterPrefab;
    public int poolSize = 5;
    [Range(0f, 10f)]
    public float spawnWeight = 1f;
}