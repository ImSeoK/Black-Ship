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
    public Vector3 hitboxBaseOffset = new Vector3(0.5f, 0f, 0f);

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
    }

    public void OnSpawn()
    {
        Debug.Log($"=== OnSpawn: {monsterName} ===");
        Debug.Log($"AttackHitbox before check: {attackHitbox != null}");

        // 컴포넌트 null이면 재초기화
        if (spriteRenderer == null || animator == null || rb == null)
        {
            Debug.LogWarning("Components null! Re-initializing...");
            InitializeComponents();
        }

        // AttackHitbox 참조 확인!
        if (attackHitbox == null)
        {
            Debug.LogWarning($"{monsterName}: AttackHitbox is NULL! Searching...");
            Transform hitboxTransform = transform.Find("AttackHitbox");
            if (hitboxTransform != null)
            {
                attackHitbox = hitboxTransform.gameObject;
                Debug.Log($"{monsterName}: AttackHitbox found and assigned!");
            }
            else
            {
                Debug.LogError($"{monsterName}: AttackHitbox NOT FOUND in children!");
            }
        }
        else
        {
            Debug.Log($"{monsterName}: AttackHitbox already assigned!");
        }

        Debug.Log($"AttackHitbox after check: {attackHitbox != null}");

        currentHealth = maxHealth;
        isDead = false;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

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

        Collider2D monsterCol = GetComponent<Collider2D>();
        Collider2D playerCol = player.GetComponent<Collider2D>();

        if (monsterCol == null || playerCol == null) return;

        // Collider 간 최단거리 계산
        float distanceToPlayer = monsterCol.Distance(playerCol).distance;

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
            float directionX = spriteRenderer.flipX ? hitboxBaseOffset.x : -hitboxBaseOffset.x;
            Vector3 targetPos = new Vector3(directionX, hitboxBaseOffset.y, hitboxBaseOffset.z);
            attackHitbox.transform.localPosition = targetPos;

            attackHitbox.SetActive(true);

            Debug.Log($"[{Time.time:F2}] {monsterName} ACTIVATE Hitbox");
        }
    }

    public void DeactivateHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
            Debug.Log($"[{Time.time:F2}] {monsterName} DEACTIVATE Hitbox");
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

        Collider2D monsterCol = GetComponent<Collider2D>();
        Collider2D playerCol = player.GetComponent<Collider2D>();

        if (monsterCol == null || playerCol == null) return;

        float distanceToPlayer = monsterCol.Distance(playerCol).distance;

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

        if (currentHealth <= 0)
        {
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
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.bodyType = RigidbodyType2D.Kinematic; // 추가!
                }
                wanderTimer = wanderWaitTime;
                break;

            case MonsterState.Wander:
                if (rb != null)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic; // 추가!
                }
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                wanderTarget = (Vector2)transform.position + randomDirection * wanderDistance;
                break;

            case MonsterState.Chase:
                if (rb != null)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic; // 추가!
                }
                break;

            case MonsterState.Attack:
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.bodyType = RigidbodyType2D.Kinematic; // 추가!
                }
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