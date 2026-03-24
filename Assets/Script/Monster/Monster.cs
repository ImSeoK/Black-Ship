using UnityEngine;
using System.Collections;

public enum MonsterState
{
    Idle,
    Wander,
    Chase,
    Attack,    // 공격 범위 안, 쿨다운 관리
    Attacking, // 공격 애니메이션 재생 중 (상태 전환 불가)
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
    private Collider2D monsterCol;
    private Collider2D playerCol;

    private int isMovingHash;
    private int attackHash;
    private int dieHash;

    private float currentDistanceToPlayer;

    private MonsterState state;
    private float currentHealth;
    private bool isDead = false;

    private float lastAttackTime = 0f;
    private float wanderTimer = 0f;
    private Vector2 wanderTarget;

    private float currentChaseSpeed;
    private Vector2 chaseOffset;

    void Start()
    {
        InitializeComponents();
    }

    void InitializeComponents()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        monsterCol = GetComponent<Collider2D>();

        isMovingHash = Animator.StringToHash("IsMoving");
        attackHash   = Animator.StringToHash("Attack");
        dieHash      = Animator.StringToHash("Die");

        if (spriteRenderer != null && sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }

        if (animator != null && animatorController != null)
        {
            animator.runtimeAnimatorController = animatorController;
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
            playerCol = player.GetComponent<Collider2D>();
    }

    public void OnSpawn()
    {
#if UNITY_EDITOR
        Debug.Log($"=== OnSpawn: {monsterName} ===");
        Debug.Log($"AttackHitbox before check: {attackHitbox != null}");
#endif

        if (spriteRenderer == null || animator == null || rb == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Components null! Re-initializing...");
#endif
            InitializeComponents();
        }

        if (attackHitbox == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{monsterName}: AttackHitbox is NULL! Searching...");
#endif
            Transform hitboxTransform = transform.Find("AttackHitbox");
            if (hitboxTransform != null)
            {
                attackHitbox = hitboxTransform.gameObject;
#if UNITY_EDITOR
                Debug.Log($"{monsterName}: AttackHitbox found and assigned!");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError($"{monsterName}: AttackHitbox NOT FOUND in children!");
#endif
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log($"{monsterName}: AttackHitbox already assigned!");
#endif
        }

#if UNITY_EDITOR
        Debug.Log($"AttackHitbox after check: {attackHitbox != null}");
#endif

        currentHealth = maxHealth;
        isDead = false;

        // 추가! 속도 차이 + 위치 오프셋
        currentChaseSpeed = chaseSpeed * Random.Range(0.8f, 1.2f);
        chaseOffset = Random.insideUnitCircle * 0.5f;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player != null)
                playerCol = player.GetComponent<Collider2D>();
        }

        ChangeState(MonsterState.Idle);
    }

    void Update()
    {
        if (state == MonsterState.Die) return;

        if (monsterCol != null && playerCol != null)
            currentDistanceToPlayer = monsterCol.Distance(playerCol).distance;

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
            case MonsterState.Attacking:
                rb.linearVelocity = Vector2.zero;
                break;
        }

        CheckPlayerDetection();
    }

    void CheckPlayerDetection()
    {
        if (player == null || monsterCol == null || playerCol == null) return;

        // 공격 애니메이션 재생 중에는 상태 전환 금지
        if (state == MonsterState.Attacking) return;

        float distanceToPlayer = currentDistanceToPlayer;

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
        }
    }

    public void DeactivateHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.SetActive(false);

        // 공격 애니메이션 종료 → 추격 재개 (죽은 경우는 Die 상태라 아무 일도 없음)
        if (state == MonsterState.Attacking)
            ChangeState(MonsterState.Chase);
    }

    void ChaseBehavior()
    {
        if (player == null || monsterCol == null || playerCol == null) return;

        float distanceToPlayer = currentDistanceToPlayer;

        // 가까우면 정확히 Player, 멀면 offset 적용
        Vector2 targetPos;
        if (distanceToPlayer > attackRange + 1f)
        {
            targetPos = (Vector2)player.position + chaseOffset;
        }
        else
        {
            targetPos = player.position;
        }

        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * currentChaseSpeed;

        FlipSprite(direction.x);
    }

    void AttackBehavior()
    {
        rb.linearVelocity = Vector2.zero;

        if (monsterCol == null || playerCol == null) return;

        float distanceToPlayer = currentDistanceToPlayer;

        if (distanceToPlayer <= attackRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                if (animator != null)
                {
                    animator.SetTrigger(attackHash);
                    ChangeState(MonsterState.Attacking);
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
#if UNITY_EDITOR
            Debug.LogWarning(monsterName + " is already dead!");
#endif
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
            animator.SetTrigger(dieHash);
        }

#if UNITY_EDITOR
        Debug.Log(monsterName + " died!");
#endif

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
                    rb.bodyType = RigidbodyType2D.Kinematic;
                }

                if (animator != null)
                    animator.SetBool(isMovingHash, false);

                wanderTimer = wanderWaitTime;
                break;

            case MonsterState.Wander:
                if (rb != null)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                }

                if (animator != null)
                    animator.SetBool(isMovingHash, true);

                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                wanderTarget = (Vector2)transform.position + randomDirection * wanderDistance;
                break;

            case MonsterState.Chase:
                if (rb != null)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                }

                if (animator != null)
                    animator.SetBool(isMovingHash, true);
                break;

            case MonsterState.Attack:
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.bodyType = RigidbodyType2D.Kinematic;
                }

                if (animator != null)
                    animator.SetBool(isMovingHash, false);
                break;

            case MonsterState.Attacking:
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.bodyType = RigidbodyType2D.Kinematic;
                }

                if (animator != null)
                    animator.SetBool(isMovingHash, false);
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