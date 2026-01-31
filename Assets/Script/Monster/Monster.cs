using UnityEngine;

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
    [Header("���� ������")]
    public MonsterData data;

    [Header("���� ����")]
    public MonsterState currentState = MonsterState.Idle;
    public float currentHealth;

    [HideInInspector]
    public MonsterSpawner spawner;

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb;

    private float attackTimer;
    private float wanderTimer;
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

        if (data != null)
        {
            spriteRenderer.sprite = data.sprite;
            if (data.animatorController != null)
                animator.runtimeAnimatorController = data.animatorController;
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public void OnSpawn()
    {
        currentHealth = data.maxHealth;
        ChangeState(MonsterState.Idle);

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (currentState == MonsterState.Die) return;

        switch (currentState)
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
        UpdateAnimations();
    }

    void CheckPlayerDetection()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= data.attackRange)
        {
            if (currentState != MonsterState.Attack)
                ChangeState(MonsterState.Attack);
        }
        else if (distanceToPlayer <= data.detectionRange)
        {
            if (currentState != MonsterState.Chase)
                ChangeState(MonsterState.Chase);
        }
        else if (distanceToPlayer > data.chaseGiveUpRange && currentState == MonsterState.Chase)
        {
            ChangeState(MonsterState.Idle);
        }
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
        rb.linearVelocity = direction * data.moveSpeed;

        FlipSprite(direction.x);

        if (Vector2.Distance(transform.position, wanderTarget) < 0.5f)
        {
            ChangeState(MonsterState.Idle);
        }
    }

    void ChaseBehavior()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * data.chaseSpeed;

        FlipSprite(direction.x);
    }

    void AttackBehavior()
    {
        rb.linearVelocity = Vector2.zero;

        if (player != null)
        {
            float directionX = player.position.x - transform.position.x;
            FlipSprite(directionX);
        }

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0)
        {
            PerformAttack();
            attackTimer = data.attackCooldown;
        }
    }

    void PerformAttack()
    {
        Debug.Log($"{data.monsterName} ����! ������: {data.attackDamage}");
        animator.SetTrigger("Attack");

        // �÷��̾� ������ ó�� (���߿� ����)
        // PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        // if (playerHealth != null)
        //     playerHealth.TakeDamage(data.attackDamage);
    }

    public void TakeDamage(float damage)
    {
        if (currentState == MonsterState.Die) return;

        currentHealth -= damage;
        Debug.Log($"{data.monsterName} ����: {damage}, ���� ü��: {currentHealth}");

        animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        ChangeState(MonsterState.Die);
        rb.linearVelocity = Vector2.zero;

        Debug.Log($"{data.monsterName} ���! ����ġ: {data.expReward}");

        animator.SetTrigger("Die");

        // ����ġ ���� (���߿� ����)
        // StatsManager.Instance.AddExp(data.expReward);

        if (spawner != null)
        {
            Invoke(nameof(ReturnToPool), 1.5f);
        }
        else
        {
            Destroy(gameObject, 1.5f);
        }
    }

    void ReturnToPool()
    {
        if (spawner != null)
            spawner.ReturnMonsterToPool(this);
    }

    void ChangeState(MonsterState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case MonsterState.Idle:
                rb.linearVelocity = Vector2.zero;
                wanderTimer = data.wanderWaitTime;
                break;

            case MonsterState.Wander:
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                wanderTarget = (Vector2)transform.position + randomDirection * data.wanderDistance;
                break;

            case MonsterState.Chase:
                break;

            case MonsterState.Attack:
                attackTimer = 0f;
                break;
        }
    }

    void UpdateAnimations()
    {
        if (animator == null) return;

        // Speed �Ķ���� (Idle/Walk ��ȯ)
        float speed = rb.linearVelocity.magnitude;
        animator.SetFloat("Speed", speed);

        // IsChasing �Ķ���� (�߰� ����)
        animator.SetBool("IsChasing", currentState == MonsterState.Chase);
    }

    void FlipSprite(float directionX)
    {
        if (directionX < 0)
            spriteRenderer.flipX = true;
        else if (directionX > 0)
            spriteRenderer.flipX = false;
    }
}