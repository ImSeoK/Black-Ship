using UnityEngine;
using System.Collections.Generic;

public class HitboxPool : MonoBehaviour
{
    public static HitboxPool Instance;

    [Header("풀 설정")]
    public int initialPoolSize = 10;

    private Queue<GameObject> boxPool = new Queue<GameObject>();
    private Queue<GameObject> circlePool = new Queue<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializePool();
    }

    void InitializePool()
    {
        // Box 히트박스 미리 생성
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject box = CreateHitbox(HitboxShape.Box);
            box.SetActive(false);
            boxPool.Enqueue(box);
        }

        // Circle 히트박스 미리 생성
        for (int i = 0; i < initialPoolSize / 2; i++)
        {
            GameObject circle = CreateHitbox(HitboxShape.Circle);
            circle.SetActive(false);
            circlePool.Enqueue(circle);
        }

        Debug.Log($"HitboxPool 초기화: Box {boxPool.Count}, Circle {circlePool.Count}");
    }

    GameObject CreateHitbox(HitboxShape shape)
    {
        GameObject hitbox = new GameObject($"Hitbox_{shape}");
        hitbox.transform.SetParent(transform);
        hitbox.layer = LayerMask.NameToLayer("PlayerAttack");

        if (shape == HitboxShape.Box)
        {
            BoxCollider2D collider = hitbox.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }
        else
        {
            CircleCollider2D collider = hitbox.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
        }

        hitbox.AddComponent<AttackHitbox>();

        return hitbox;
    }

    public GameObject Get(HitboxShape shape)
    {
        Queue<GameObject> pool = shape == HitboxShape.Box ? boxPool : circlePool;

        GameObject hitbox;

        if (pool.Count > 0)
        {
            hitbox = pool.Dequeue();
        }
        else
        {
            hitbox = CreateHitbox(shape);
            Debug.Log($"풀 부족, 새로 생성: {shape}");
        }

        hitbox.SetActive(true);
        return hitbox;
    }

    public void Return(GameObject hitbox, HitboxShape shape)
    {
        if (hitbox == null) return;

        hitbox.SetActive(false);
        hitbox.transform.SetParent(transform);

        Queue<GameObject> pool = shape == HitboxShape.Box ? boxPool : circlePool;
        pool.Enqueue(hitbox);
    }
}