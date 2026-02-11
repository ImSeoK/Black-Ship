using UnityEngine;

public class MonsterAttackHitbox : MonoBehaviour
{
    private Monster monster;
    private BoxCollider2D hitboxCollider;
    private bool hasDealtDamage = false;

    void Awake()
    {
        monster = GetComponentInParent<Monster>();
        hitboxCollider = GetComponent<BoxCollider2D>();
    }

    // Start() 삭제! Pool에서 이미 비활성화됨

    void OnEnable()
    {
        hasDealtDamage = false;
        CheckOverlap();
    }

    void CheckOverlap()
    {
        if (hitboxCollider == null || hasDealtDamage) return;

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            hitboxCollider.bounds.center,
            hitboxCollider.bounds.size,
            0f
        );

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player") && !hasDealtDamage)
            {
                DealDamage(hit);
                return;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasDealtDamage) return;

        if (other.CompareTag("Player"))
        {
            DealDamage(other);
        }
    }

    void DealDamage(Collider2D playerCollider)
    {
        if (hasDealtDamage) return;

        if (StatsManager.Instance != null && monster != null)
        {
            StatsManager.Instance.TakeDamage(monster.attackDamage);
            hasDealtDamage = true;
        }
    }

    void OnDrawGizmos()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.offset, box.size);

            Gizmos.color = new Color(1f, 0.5f, 0f, 1f);
            Gizmos.DrawWireCube(box.offset, box.size);
        }
    }
}