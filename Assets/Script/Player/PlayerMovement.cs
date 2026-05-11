using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement - HOT RELOAD")]
    public float moveSpeed = 4f;
    public float runSpeed = 8f;

    [Header("Dodge Roll - HOT RELOAD")]
    public float rollSpeed = 12f;
    public float rollDuration = 0.3f;
    public float rollCooldown = 1f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

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

        // 공격 중이면 입력 무시 (추가!)
        if (AttackController.Instance != null && AttackController.Instance.IsAttacking())
        {
            moveInput = Vector2.zero;
            return;
        }

        // 구르기 중이 아닐 때만 입력 받기
        if (!isRolling)
        {
            moveInput = InputManager.Instance != null ? InputManager.Instance.MoveInput : Vector2.zero;
        }

        bool isRunning = InputManager.Instance != null && InputManager.Instance.IsRunning;

        // 구르기 쿨다운 감소
        if (rollCooldownTimer > 0)
        {
            rollCooldownTimer -= Time.deltaTime;
        }

        // 구르기 입력 확인
        if (InputManager.Instance != null && InputManager.Instance.IsRollPressed && !isRolling && rollCooldownTimer <= 0 && moveInput.magnitude > 0)
        {
            StartRoll();
        }

        // 구르기 타이머 처리
        if (isRolling)
        {
            rollTimer -= Time.deltaTime;
            if (rollTimer <= 0)
            {
                EndRoll();
            }
        }

        // 애니메이션
        if (animator != null)
        {
            bool isMoving = moveInput.magnitude > 0 && !isRolling;
            animator.SetBool("isMoving", isMoving);
            //animator.SetBool("IsRolling", isRolling);

            float baseSpeed = isRunning ? runSpeed : moveSpeed;

            float speedModifier = 1f;
            if (BabyManager.Instance != null && BabyManager.Instance.carryingBaby)
            {
                speedModifier = 0.7f;
            }

            float currentSpeed = baseSpeed * speedModifier;

            animator.SetFloat("speed", isMoving ? currentSpeed : 0);

            // 좌우 반전
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

        // 공격 중이면 이동 중지 (추가!)
        if (AttackController.Instance != null && AttackController.Instance.IsAttacking())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isRolling)
        {
            rb.linearVelocity = rollDirection * rollSpeed;
        }
        else
        {
            bool isRunning = InputManager.Instance != null && InputManager.Instance.IsRunning;

            float baseSpeed = isRunning ? runSpeed : moveSpeed;

            float speedModifier = 1f;
            if (BabyManager.Instance != null && BabyManager.Instance.carryingBaby)
            {
                speedModifier = 0.7f;
            }

            float currentSpeed = baseSpeed * speedModifier;

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