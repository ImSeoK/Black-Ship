using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat Instance;

    [Header("Combat Settings")]
    public float invincibilityTime = 1f;
    private float lastDamageTime = -999f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        if (Time.time < lastDamageTime + invincibilityTime)
        {
            Debug.Log("Player is invincible!");
            return;
        }

        lastDamageTime = Time.time;

        PlayerStats stats = PlayerStats.Instance;
        if (stats == null) return;

        stats.AddHP(-damage);

        if (stats.playerHP <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("Player died!");
    }
}