using UnityEngine;
using System.Collections.Generic;

public class MonsterSpawner : MonoBehaviour
{
    public static MonsterSpawner Instance;

    [Header("스폰 설정")]
    public List<MonsterSpawnData> monsterTypes;
    public int maxMonsters = 10;
    public float spawnInterval = 5f;

    [Header("스폰 영역")]
    public Vector2 spawnAreaMin = new Vector2(-20, -20);
    public Vector2 spawnAreaMax = new Vector2(20, 20);
    public float minDistanceFromPlayer = 10f;

    private List<Monster> activeMonsters = new List<Monster>();
    private Dictionary<MonsterData, Queue<Monster>> monsterPools = new Dictionary<MonsterData, Queue<Monster>>();
    private float spawnTimer;
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

        InitializePools();

        // 초기 스폰
        for (int i = 0; i < maxMonsters / 2; i++)
        {
            SpawnRandomMonster();
        }
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval && activeMonsters.Count < maxMonsters)
        {
            SpawnRandomMonster();
            spawnTimer = 0f;
        }
    }

    void InitializePools()
    {
        foreach (var spawnData in monsterTypes)
        {
            Queue<Monster> pool = new Queue<Monster>();

            for (int i = 0; i < spawnData.poolSize; i++)
            {
                Monster monster = CreateMonster(spawnData.monsterData);
                monster.gameObject.SetActive(false);
                pool.Enqueue(monster);
            }

            monsterPools[spawnData.monsterData] = pool;
        }
    }

    Monster CreateMonster(MonsterData data)
    {

        GameObject monsterObj = new GameObject(data.monsterName);
        monsterObj.transform.SetParent(transform);

        // Layer 설정 추가!
        monsterObj.layer = LayerMask.NameToLayer("Enemy");

        // Sprite Renderer
        SpriteRenderer sr = monsterObj.AddComponent<SpriteRenderer>();
        sr.sprite = data.sprite;
        sr.sortingOrder = 5;

        // Animator
        Animator animator = monsterObj.AddComponent<Animator>();
        if (data.animatorController != null)
            animator.runtimeAnimatorController = data.animatorController;

        // Rigidbody2D
        Rigidbody2D rb = monsterObj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Collider
        CircleCollider2D collider = monsterObj.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;

        // YAxisSorting (있다면)
        //YAxisSorting yAxis = monsterObj.AddComponent<YAxisSorting>();
        //yAxis.sortingOrderBase = 5000;
        //yAxis.isStatic = false;

        // Monster 스크립트
        Monster monster = monsterObj.AddComponent<Monster>();
        monster.data = data;
        monster.spawner = this;

        return monster;
    }

    public void SpawnRandomMonster()
    {
        if (monsterTypes.Count == 0) return;

        // 가중치 기반 랜덤 선택
        float totalWeight = 0f;
        foreach (var data in monsterTypes)
            totalWeight += data.spawnWeight;

        float randomValue = Random.Range(0f, totalWeight);
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

        Monster monster = GetMonsterFromPool(selectedData.monsterData);

        if (monster != null)
        {
            Vector2 spawnPos = GetRandomSpawnPosition();
            monster.transform.position = spawnPos;

            monster.gameObject.SetActive(true);
            monster.OnSpawn();

            activeMonsters.Add(monster);

            Debug.Log($"{selectedData.monsterData.monsterName} 스폰: {spawnPos}");
        }
    }

    Monster GetMonsterFromPool(MonsterData data)
    {
        if (monsterPools.ContainsKey(data) && monsterPools[data].Count > 0)
        {
            return monsterPools[data].Dequeue();
        }
        else
        {
            return CreateMonster(data);
        }
    }

    public void ReturnMonsterToPool(Monster monster)
    {
        if (monster == null) return;

        activeMonsters.Remove(monster);
        monster.gameObject.SetActive(false);

        if (monsterPools.ContainsKey(monster.data))
        {
            monsterPools[monster.data].Enqueue(monster);
        }
    }

    Vector2 GetRandomSpawnPosition()
    {
        Vector2 spawnPos;
        int attempts = 0;

        do
        {
            float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
            spawnPos = new Vector2(x, y);
            attempts++;

        } while (player != null &&
                 Vector2.Distance(spawnPos, player.transform.position) < minDistanceFromPlayer &&
                 attempts < 20);

        return spawnPos;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector2 center = (spawnAreaMin + spawnAreaMax) / 2f;
        Vector2 size = spawnAreaMax - spawnAreaMin;
        Gizmos.DrawWireCube(center, size);
    }
}

[System.Serializable]
public class MonsterSpawnData
{
    public MonsterData monsterData;
    public int poolSize = 5;
    [Range(0f, 1f)]
    public float spawnWeight = 1f;
}
