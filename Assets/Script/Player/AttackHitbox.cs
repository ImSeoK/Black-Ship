using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    private float damage;
    private string attackName;

    public void Initialize(float dmg, string name)
    {
        damage = dmg;
        attackName = name;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(">>> TRIGGER ENTER: " + collision.name +
                  " (Tag: " + collision.tag + ")");

        if (collision.CompareTag("Enemy"))
        {
            Monster monster = collision.GetComponent<Monster>();
            if (monster != null)
            {
                monster.TakeDamage(damage);
                Debug.Log(">>> HIT: " + monster.monsterName + " for " + damage);
            }
            else
            {
                Debug.LogError("No Monster component!");
            }
        }
    }

    // Play 모드에서 히트박스 보이기
    void OnDrawGizmos()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f); // 반투명 빨강
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.offset, box.size);

            Gizmos.color = Color.red; // 테두리
            Gizmos.DrawWireCube(box.offset, box.size);
        }

        CircleCollider2D circle = GetComponent<CircleCollider2D>();
        if (circle != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawSphere(transform.position + (Vector3)circle.offset, circle.radius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + (Vector3)circle.offset, circle.radius);
        }
    }
}
