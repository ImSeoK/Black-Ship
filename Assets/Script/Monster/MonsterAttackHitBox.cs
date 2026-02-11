using UnityEngine;

public class MonsterAttackHitbox : MonoBehaviour
{
    private Monster monster;

    void Awake()
    {
        monster = GetComponentInParent<Monster>();

        // 시작 시 비활성화
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Player만 공격
        if (other.CompareTag("Player"))
        {
            if (StatsManager.Instance != null && monster != null)
            {
                StatsManager.Instance.TakeDamage(monster.attackDamage);
                Debug.Log($"{monster.monsterName} hit player for {monster.attackDamage} damage!");
            }
        }
    }

    // Gizmo로 Hitbox 영역 표시
    void OnDrawGizmos()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f); // 주황색
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.offset, box.size);

            Gizmos.color = new Color(1f, 0.5f, 0f, 1f);
            Gizmos.DrawWireCube(box.offset, box.size);
        }
    }
}