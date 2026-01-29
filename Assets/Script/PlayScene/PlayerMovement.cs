using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement - HOT RELOAD ����")]
    public float moveSpeed = 4f;
    public float runSpeed = 8f; // �߰�!

    [Header("Dodge Roll - HOT RELOAD ����")]
    public float rollSpeed = 12f;
    public float rollDuration = 0.3f;
    public float rollCooldown = 1f;

    [Header("Input Settings")]
    [SerializeField] private KeyCode rollKey = KeyCode.Space;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    // ������ ���� ����
    private bool isRolling = false;
    private float rollTimer = 0f;
    private float rollCooldownTimer = 0f;
    private Vector2 rollDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.timeScale == 0) return;

        // ������ ���� �ƴ� ���� �Է� �ޱ�
        if (!isRolling)
        {
            moveInput.x = 0;
            moveInput.y = 0;
            if (Input.GetKey(KeyCode.A)) moveInput.x = -1;
            if (Input.GetKey(KeyCode.D)) moveInput.x = 1;
            if (Input.GetKey(KeyCode.W)) moveInput.y = 1;
            if (Input.GetKey(KeyCode.S)) moveInput.y = -1;
            moveInput.Normalize();
        }

        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // ������ ��ٿ� ����
        if (rollCooldownTimer > 0)
        {
            rollCooldownTimer -= Time.deltaTime;
        }

        // ������ �Է� Ȯ��
        if (Input.GetKeyDown(rollKey) && !isRolling && rollCooldownTimer <= 0 && moveInput.magnitude > 0)
        {
            StartRoll();
        }

        // ������ Ÿ�̸� ó��
        if (isRolling)
        {
            rollTimer -= Time.deltaTime;
            if (rollTimer <= 0)
            {
                EndRoll();
            }
        }

        // �ִϸ��̼�
        if (animator != null)
        {
            bool isMoving = moveInput.magnitude > 0 && !isRolling;
            animator.SetBool("isMoving", isMoving);
            animator.SetBool("IsRolling", isRolling);

            // ===== ���� �κ�: �ӵ� ��� =====
            float baseSpeed = isRunning ? runSpeed : moveSpeed;

            // ���� ��� ������ �ӵ� ����
            float speedModifier = 1f;
            if (StatsManager.Instance != null && StatsManager.Instance.carryingBaby)
            {
                speedModifier = 0.7f; // 30% ������
            }

            float currentSpeed = baseSpeed * speedModifier;
            // ===== ���� �� =====

            animator.SetFloat("speed", isMoving ? currentSpeed : 0);

            // �¿� ����
            if (moveInput.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (moveInput.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }


void FixedUpdate()
{
    if (Time.timeScale == 0) return;

    if (isRolling)
    {
        // ������ �� �̵�
        rb.linearVelocity = rollDirection * rollSpeed;
    }
    else
    {
        // �Ϲ� �̵�
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // ===== ���� �κ� =====
        float baseSpeed = isRunning ? runSpeed : moveSpeed;

        // ���� ��� ������ �ӵ� ����
        float speedModifier = 1f;
        if (StatsManager.Instance != null && StatsManager.Instance.carryingBaby)
        {
            speedModifier = 0.7f;
        }

        float currentSpeed = baseSpeed * speedModifier;
        // ===== ���� �� =====

        rb.linearVelocity = moveInput * currentSpeed;
    }
}

private void StartRoll()
    {
        isRolling = true;
        rollTimer = rollDuration;
        rollCooldownTimer = rollCooldown;
        rollDirection = moveInput;
    }

    private void EndRoll()
    {
        isRolling = false;
    }

    public bool IsRolling()
    {
        return isRolling;
    }
}