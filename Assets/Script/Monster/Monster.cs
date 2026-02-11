using UnityEngine;
using System.Collections;

public enum MonsterState
{
    Idle,
    Wander,
    Chase,
    Attack,
    Die
}

public class Monster : MonoBehaviour
{
    [Header("Attack Hitbox")]
    public GameObject attackHitbox;

    [Header("Monster Info")]
    public string monsterName = "Slime";
    public Sprite sprite;
    public RuntimeAnimatorController animatorController;

    [Header("Stats")]
    public float maxHealth = 50f;
    public float moveSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float attackDamage = 10f;

    [Header("Detection")]
    public float detectionRange = 8f;
    public float attackRange = 1.5f;
    public float chaseGiveUpRange = 15f;
    public float attackCooldown = 2f;

    [Header("Wander")]
    public float wanderDistance = 5f;
    public float wanderWaitTime = 3f;

    [Header("References")]
    public MonsterSpawner spawner;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private MonsterState state;
    private float currentHealth;
    private bool isDead = false;

    private float lastAttackTime = 0f;
    private float wanderTimer = 0f;
    private Vector2 wanderTarget;

    void Start()
    {
        InitializeComponents();
    }

    void InitializeComponents()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer != null && sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }

        if (animator != null && animatorController != null)
        {
            animator.runtimeAnimatorController = animatorController;
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        Debug.Log("InitializeComponents: SpriteRenderer=" + (spriteRenderer != null) +
                  ", Animator=" + (animator != null) +
                  ", RB=" + (rb != null));
    }

    public void OnSpawn()
    {
        // 컴포넌트 null이면 재초기화
        if (spriteRenderer == null || animator == null || rb == null)
        {
            Debug.LogWarning("Components null! Re-initializing...");
            InitializeComponents();
        }

        currentHealth = maxHealth;
        isDead = false;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        Debug.Log("=== OnSpawn ===" +
                  "\nMonster: " + monsterName +
                  "\nHP: " + currentHealth +
                  "\nRB: " + (rb != null) +
                  "\nAnimator: " + (animator != null) +
                  "\nPlayer: " + (player != null));

        ChangeState(MonsterState.Idle);
    }

    void Update()
    {
        if (state == MonsterState.Die) return;

        switch (state)
        {
            case MonsterState.Idle:
                IdleBehavior();
                break;
            case MonsterState.Wander:
                WanderBehavior();
                break;
            case MonsterState.Chase:
                ChaseBehavior();
                break;
            case MonsterState.Attack:
                AttackBehavior();
                break;
        }

        CheckPlayerDetection();
    }

    void CheckPlayerDetection()
    {
        if (player == null) return;

        // Collider 중심 기준으로 거리 계산
        Collider2D monsterCol = GetComponent<Collider2D>();
        Collider2D playerCol = player.GetComponent<Collider2D>();

        Vector2 monsterCenter = monsterCol != null ? monsterCol.bounds.center : transform.position;
        Vector2 playerCenter = playerCol != null ? playerCol.bounds.center : player.position;

        float distanceToPlayer = Vector2.Distance(monsterCenter, playerCenter);

        if (distanceToPlayer <= attackRange)
        {
            if (state != MonsterState.Attack)
                ChangeState(MonsterState.Attack);
        }
        else if (distanceToPlayer <= detectionRange)
        {
            if (state != MonsterState.Chase)
                ChangeState(MonsterState.Chase);
        }
        else if (distanceToPlayer > chaseGiveUpRange && state == MonsterState.Chase)
        {
            ChangeState(MonsterState.Idle);
        }
    }

    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }

    void IdleBehavior()
    {
        rb.linearVelocity = Vector2.zero;
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0)
        {
            ChangeState(MonsterState.Wander);
        }
    }

    void WanderBehavior()
    {
        Vector2 direction = (wanderTarget - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        FlipSprite(direction.x);

        if (Vector2.Distance(transform.position, wanderTarget) < 0.5f)
        {
            ChangeState(MonsterState.Idle);
        }
    }

    public void ActivateHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(true);
            Debug.Log($"{monsterName} Hitbox Activated");
        }
    }

    public void DeactivateHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
            Debug.Log($"{monsterName} Hitbox Deactivated");
        }
    }

    void ChaseBehavior()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * chaseSpeed;

        FlipSprite(direction.x);
    }

    void AttackBehavior()
    {
        rb.linearVelocity = Vector2.zero;

        // Collider 중심 기준
        Collider2D monsterCol = GetComponent<Collider2D>();
        Collider2D playerCol = player.GetComponent<Collider2D>();

        Vector2 monsterCenter = monsterCol != null ? monsterCol.bounds.center : transform.position;
        Vector2 playerCenter = playerCol != null ? playerCol.bounds.center : player.position;

        float distanceToPlayer = Vector2.Distance(monsterCenter, playerCenter);

        if (distanceToPlayer <= attackRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                if (animator != null)
                {
                    animator.SetTrigger("Attack");
                }
                lastAttackTime = Time.time;
            }
        }
        else
        {
            ChangeState(MonsterState.Chase);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
        {
            Debug.LogWarning(monsterName + " is already dead!");
            return;
        }

        currentHealth -= damage;

        Debug.Log(">>> HIT: " + monsterName +
                  " took " + damage + " damage. HP: " +
                  currentHealth + "/" + maxHealth);

        if (currentHealth <= 0)
        {
            Debug.Log(">>> DYING: " + monsterName + " HP reached 0!");
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        Debug.Log(monsterName + " died!");

        StartCoroutine(DieAfterAnimation());
    }

    IEnumerator DieAfterAnimation()
    {
        // 애니메이션 길이에 맞추기 (나중에 사용)
        // AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // float animLength = stateInfo.length;
        // yield return new WaitForSeconds(animLength);

        // 즉시 사라지기
        yield return null;

        if (spawner != null)
        {
            spawner.ReturnMonsterToPool(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void ChangeState(MonsterState newState)
    {
        state = newState;

        switch (newState)
        {
            case MonsterState.Idle:
                if (rb != null) // null 체크 추가!
                {
                    rb.linearVelocity = Vector2.zero;
                }
                wanderTimer = wanderWaitTime;
                break;

            case MonsterState.Wander:
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                wanderTarget = (Vector2)transform.position + randomDirection * wanderDistance;
                break;
        }
    }

    void FlipSprite(float directionX)
    {
        if (directionX > 0)
            spriteRenderer.flipX = true; // 오른쪽 가려면 반전!
        else if (directionX < 0)
            spriteRenderer.flipX = false; // 왼쪽은 원본 그대로
    }
}