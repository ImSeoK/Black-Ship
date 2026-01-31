using UnityEngine;
using System.Collections.Generic;

public class AttackHitbox : MonoBehaviour
{
    public float damage = 10f;
    public string attackName = "공격";

    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();

    // Object Pooling용 초기화
    public void Initialize(float dmg, string name)
    {
        damage = dmg;
        attackName = name;
        hitEnemies.Clear();
    }

    // Object Pooling용 리셋
    public void ResetHitbox()
    {
        damage = 0f;
        attackName = "";
        hitEnemies.Clear();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hitEnemies.Contains(other)) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Monster monster = other.GetComponent<Monster>();
            if (monster != null)
            {
                monster.TakeDamage(damage);
                hitEnemies.Add(other);

                Debug.Log($" 【{attackName}】 {monster.data.monsterName}에게 {damage:F0} 데미지!");
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);

        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.offset, box.size);
        }

        CircleCollider2D circle = GetComponent<CircleCollider2D>();
        if (circle != null)
        {
            Gizmos.DrawSphere(transform.position + (Vector3)circle.offset, circle.radius);
        }
    }
}