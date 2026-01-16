using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement - HOT RELOAD 가능")]
    public float moveSpeed = 4f;
    public float runSpeed = 8f; // 추가!

    [Header("Dodge Roll - HOT RELOAD 가능")]
    public float rollSpeed = 12f;
    public float rollDuration = 0.3f;
    public float rollCooldown = 1f;

    [Header("Input Settings")]
    [SerializeField] private KeyCode rollKey = KeyCode.Space;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    // 구르기 상태 변수
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

        // 구르기 중이 아닐 때만 입력 받기
        if (!isRolling)
        {
            // 방향키만 사용
            moveInput.x = 0;
            moveInput.y = 0;
            if (Input.GetKey(KeyCode.LeftArrow)) moveInput.x = -1;
            if (Input.GetKey(KeyCode.RightArrow)) moveInput.x = 1;
            if (Input.GetKey(KeyCode.UpArrow)) moveInput.y = 1;
            if (Input.GetKey(KeyCode.DownArrow)) moveInput.y = -1;
            moveInput.Normalize();
        }

        // Shift 확인
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // 구르기 쿨다운 감소
        if (rollCooldownTimer > 0)
        {
            rollCooldownTimer -= Time.deltaTime;
        }

        // 구르기 입력 확인
        if (Input.GetKeyDown(rollKey) && !isRolling && rollCooldownTimer <= 0 && moveInput.magnitude > 0)
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
            animator.SetBool("IsRolling", isRolling);

            // 속도 파라미터 (Walk/Run 구분)
            float currentSpeed = isRunning ? runSpeed : moveSpeed;
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
        if (isRolling)
        {
            // 구르기 중에는 구르기 방향으로 빠르게 이동
            rb.MovePosition(rb.position + rollDirection * rollSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Shift에 따라 속도 변경
            bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            float currentSpeed = isRunning ? runSpeed : moveSpeed;

            rb.MovePosition(rb.position + moveInput * currentSpeed * Time.fixedDeltaTime);
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