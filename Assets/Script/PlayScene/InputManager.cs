using UnityEngine;

/// <summary>
/// 모든 입력을 한 곳에서 관리합니다.
/// [DefaultExecutionOrder(-100)] 으로 다른 스크립트보다 먼저 Update가 실행됩니다.
/// 키 변경은 이 컴포넌트의 Inspector에서만 수정하면 됩니다.
/// </summary>
[DefaultExecutionOrder(-100)]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [Header("이동")]
    public KeyCode moveLeft  = KeyCode.A;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode moveUp    = KeyCode.W;
    public KeyCode moveDown  = KeyCode.S;

    [Header("액션")]
    public KeyCode rollKey      = KeyCode.Space;
    public KeyCode interactKey  = KeyCode.E;
    public KeyCode inventoryKey = KeyCode.I;
    public KeyCode pauseKey     = KeyCode.Escape;

    [Header("공격")]
    public bool useMouseInput     = true;
    public KeyCode basicAttackKey = KeyCode.Space;
    public KeyCode skill1Key      = KeyCode.Q;
    public KeyCode skill2Key      = KeyCode.E;

    // ── 이동 ──────────────────────────────────────────
    /// <summary>WASD 입력을 정규화한 벡터</summary>
    public Vector2 MoveInput { get; private set; }
    /// <summary>Shift 홀드 여부</summary>
    public bool IsRunning { get; private set; }

    // ── 액션 (해당 프레임만 true) ─────────────────────
    public bool IsRollPressed      { get; private set; }
    public bool IsInteractPressed  { get; private set; }
    public bool IsInventoryPressed { get; private set; }
    public bool IsPausePressed     { get; private set; }

    // ── 공격 (해당 프레임만 true) ─────────────────────
    public bool IsBasicAttackPressed { get; private set; }
    public bool IsSkill1Pressed      { get; private set; }
    public bool IsSkill2Pressed      { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // 이동 벡터
        float x = 0f, y = 0f;
        if (Input.GetKey(moveLeft))  x = -1f;
        if (Input.GetKey(moveRight)) x =  1f;
        if (Input.GetKey(moveUp))    y =  1f;
        if (Input.GetKey(moveDown))  y = -1f;
        MoveInput = new Vector2(x, y).normalized;

        IsRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // 액션
        IsRollPressed      = Input.GetKeyDown(rollKey);
        IsInteractPressed  = Input.GetKeyDown(interactKey);
        IsInventoryPressed = Input.GetKeyDown(inventoryKey);
        IsPausePressed     = Input.GetKeyDown(pauseKey);

        // 공격
        if (useMouseInput)
        {
            IsBasicAttackPressed = Input.GetMouseButtonDown(0);
            IsSkill1Pressed      = Input.GetMouseButtonDown(1);
            IsSkill2Pressed      = Input.GetMouseButtonDown(2);
        }
        else
        {
            IsBasicAttackPressed = Input.GetKeyDown(basicAttackKey);
            IsSkill1Pressed      = Input.GetKeyDown(skill1Key);
            IsSkill2Pressed      = Input.GetKeyDown(skill2Key);
        }
    }
}
