using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackController : MonoBehaviour
{
    public static AttackController Instance;

    [Header("Attack Data (Auto Set)")]
    public List<AttackData> attacks = new List<AttackData>();

    [Header("Input Settings")]
    public bool useMouseInput = true;
    public KeyCode basicAttackKey = KeyCode.Space;
    public KeyCode skill1Key = KeyCode.Q;
    public KeyCode skill2Key = KeyCode.E;

    [Header("References")]
    public Transform hitboxParent;
    public Transform effectParent;

    private CameraFollow cameraFollow;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isAttacking = false;
    private bool hasWeapon = false;
    private Dictionary<AttackData, float> lastAttackTimes = new Dictionary<AttackData, float>();

    private AttackData currentAttack;
    private Vector2 pendingPositionShift = Vector2.zero;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void OnEnable()
    {
        FindCameraFollow();
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (hitboxParent == null) hitboxParent = transform;
        if (effectParent == null) effectParent = transform;

        FindCameraFollow();

        LoadWeaponFromManager();
    }

    void LoadWeaponFromManager()
    {
        if (WeaponManager.Instance == null)
        {
            Debug.LogError("WeaponManager not found!");
            return;
        }

        WeaponAttackSet attackSet = WeaponManager.Instance.currentAttackSet;

        if (attackSet == null)
        {
            Debug.LogWarning("No attack set equipped!");
            hasWeapon = false;
            return;
        }

        attacks.Clear();
        lastAttackTimes.Clear();

        foreach (var attack in attackSet.attacks)
        {
            if (attack != null)
            {
                attacks.Add(attack);
                lastAttackTimes[attack] = -999f;

                if (!HasAnimationTrigger(attack.animationTrigger))
                {
                    Debug.LogError("Animation trigger missing: " + attack.animationTrigger);
                }
            }
        }

        hasWeapon = attacks.Count > 0;

        Debug.Log("Attack patterns loaded: " + attackSet.weaponType + " (" + attacks.Count + " attacks)");
    }

    void FindCameraFollow()
    {
        cameraFollow = Object.FindAnyObjectByType<CameraFollow>();

        if (cameraFollow == null)
        {
            Debug.LogWarning("CameraFollow not found! Camera won't move during skills.");
        }
        else
        {
            Debug.Log("CameraFollow found on: " + cameraFollow.gameObject.name);
        }
    }

    bool HasAnimationTrigger(string triggerName)
    {
        if (animator == null) return false;

        foreach (var param in animator.parameters)
        {
            if (param.name == triggerName && param.type == AnimatorControllerParameterType.Trigger)
            {
                return true;
            }
        }
        return false;
    }

    public void ReloadWeapon()
    {
        LoadWeaponFromManager();
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    void Update()
    {
        if (isAttacking || !hasWeapon) return;

        if (useMouseInput)
        {
            if (Input.GetMouseButtonDown(0) && attacks.Count > 0)
            {
                TryAttack(attacks[0]);
            }

            if (Input.GetMouseButtonDown(1) && attacks.Count > 1)
            {
                TryAttack(attacks[1]);
            }

            if (Input.GetMouseButtonDown(2) && attacks.Count > 2)
            {
                TryAttack(attacks[2]);
            }
        }
        else
        {
            if (Input.GetKeyDown(basicAttackKey) && attacks.Count > 0)
            {
                TryAttack(attacks[0]);
            }

            if (Input.GetKeyDown(skill1Key) && attacks.Count > 1)
            {
                TryAttack(attacks[1]);
            }

            if (Input.GetKeyDown(skill2Key) && attacks.Count > 2)
            {
                TryAttack(attacks[2]);
            }
        }
    }

    void TryAttack(AttackData attackData)
    {
        if (attackData == null)
        {
            Debug.LogError("AttackData is null!");
            return;
        }

        float cooldown = WeaponManager.Instance.CalculateFinalCooldown(attackData.cooldown);

        if (Time.time < lastAttackTimes[attackData] + cooldown)
        {
            Debug.Log(attackData.attackName + " on cooldown! (" + cooldown.ToString("F1") + "s)");
            return;
        }

        StartCoroutine(ExecuteAttack(attackData));
    }

    IEnumerator ExecuteAttack(AttackData attackData)
    {
        isAttacking = true;
        lastAttackTimes[attackData] = Time.time;
        currentAttack = attackData;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        Debug.Log("[" + attackData.attackName + "] Execute!");

        animator.SetTrigger(attackData.animationTrigger);

        if (attackData.isDashSkill)
        {
            yield return StartCoroutine(ExecuteDash(attackData));
        }
        else
        {
            if (attackData.timingMode == TimingMode.Time)
            {
                if (attackData.hasEffect && attackData.effectPrefab != null)
                {
                    StartCoroutine(SpawnEffectDelayed(attackData, attackData.effectSpawnTime));
                }

                StartCoroutine(SpawnHitboxDelayed(attackData, attackData.hitboxActiveTime));
            }

            // shiftsPosition이면 카메라 이동, 아니면 제자리
            if (attackData.shiftsPosition)
            {
                yield return StartCoroutine(MoveCameraWithSkill(attackData));
            }
            else
            {
                // 제자리 공격
                float elapsed = 0f;
                while (elapsed < attackData.animationDuration)
                {
                    if (rb != null)
                    {
                        rb.linearVelocity = Vector2.zero;
                    }
                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }
        }

        isAttacking = false;
        currentAttack = null;
    }

    IEnumerator ExecuteDash(AttackData attackData)
    {
        float direction = transform.localScale.x < 0 ? -1f : 1f;
        Vector2 dashVector = new Vector2(attackData.dashDistance * direction, 0);

        Vector2 startPos = transform.position;
        Vector2 endPos = startPos + dashVector;

        if (attackData.checkObstacles)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                startPos,
                dashVector.normalized,
                attackData.dashDistance,
                LayerMask.GetMask("Obstacle", "Wall")
            );

            if (hit.collider != null)
            {
                float safeDistance = Mathf.Max(0, hit.distance - 0.5f);
                endPos = startPos + dashVector.normalized * safeDistance;
            }
        }

        if (attackData.timingMode == TimingMode.Time)
        {
            CreatePathHitbox(startPos, endPos, attackData.hitboxDuration);
        }

        float duration = attackData.animationDuration;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.position = Vector2.Lerp(startPos, endPos, t);

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            yield return null;
        }

        transform.position = endPos;
    }

    IEnumerator MoveCameraWithSkill(AttackData attackData)
    {
        float direction = transform.localScale.x < 0 ? -1f : 1f;

        Vector2 cameraShift = new Vector2(
            attackData.positionShift.x * direction,
            attackData.positionShift.y
        );

        // 이동량 저장 (OnAttackEnd에서 사용)
        pendingPositionShift = cameraShift;

        float duration = attackData.animationDuration;
        float elapsed = 0f;

        if (cameraFollow == null)
        {
            FindCameraFollow();
        }

        if (cameraFollow == null)
        {
            Debug.LogError("CameraFollow not found in scene!");
            yield break;
        }

        cameraFollow.ResetTempOffset();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Vector3 currentOffset = Vector2.Lerp(Vector2.zero, cameraShift, t);

            cameraFollow.SetTempOffset(currentOffset);

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            yield return null;
        }
    }

    void CreatePathHitbox(Vector2 startPos, Vector2 endPos, float duration)
    {
        Vector2 center = (startPos + endPos) / 2f;
        float distance = Vector2.Distance(startPos, endPos);

        GameObject hitboxObj = new GameObject(currentAttack.attackName + "_PathHitbox");
        hitboxObj.transform.position = center;
        hitboxObj.transform.SetParent(hitboxParent);
        hitboxObj.layer = LayerMask.NameToLayer("PlayerAttack");

        BoxCollider2D box = hitboxObj.AddComponent<BoxCollider2D>();
        box.size = new Vector2(distance, 1f);
        box.isTrigger = true;

        float angle = Mathf.Atan2(endPos.y - startPos.y, endPos.x - startPos.x) * Mathf.Rad2Deg;
        hitboxObj.transform.rotation = Quaternion.Euler(0, 0, angle);

        AttackHitbox hitbox = hitboxObj.AddComponent<AttackHitbox>();
        float finalDamage = WeaponManager.Instance.CalculateFinalDamage(currentAttack.damage);
        hitbox.Initialize(finalDamage, currentAttack.attackName);

        Destroy(hitboxObj, duration);
    }

    public void OnAttackHitboxSpawn()
    {
        if (currentAttack == null) return;
        SpawnHitboxNow(currentAttack);
    }

    public void OnAttackEffectSpawn()
    {
        if (currentAttack == null) return;
        SpawnEffectNow(currentAttack);
    }

    public void OnAttackEnd()
    {
        Debug.Log("=== OnAttackEnd START ===");
        Debug.Log("pendingPositionShift: " + pendingPositionShift);
        Debug.Log("Player pos BEFORE: " + transform.position);

        if (pendingPositionShift.magnitude > 0.01f)
        {
            // 카메라 오프셋 먼저 리셋
            if (cameraFollow != null)
            {
                Debug.Log("Resetting camera offset");
                cameraFollow.ResetTempOffset();
            }

            // Transform 이동
            transform.position = (Vector2)transform.position + pendingPositionShift;
            Debug.Log("Player pos AFTER: " + transform.position);

            pendingPositionShift = Vector2.zero;
        }

        Debug.Log("=== OnAttackEnd END ===");

        isAttacking = false;
        currentAttack = null;
    }

    void SpawnHitboxNow(AttackData attackData)
    {
        if (HitboxPool.Instance == null)
        {
            Debug.LogError("HitboxPool not found!");
            return;
        }

        float directionMultiplier = transform.localScale.x < 0 ? -1f : 1f;
        Vector2 offset = new Vector2(
            attackData.hitboxOffset.x * directionMultiplier,
            attackData.hitboxOffset.y
        );

        Vector2 spawnPos = (Vector2)hitboxParent.position + offset;

        GameObject hitboxObj = HitboxPool.Instance.Get(attackData.hitboxShape);
        hitboxObj.transform.position = spawnPos;
        hitboxObj.transform.SetParent(hitboxParent);

        if (attackData.hitboxShape == HitboxShape.Box)
        {
            BoxCollider2D box = hitboxObj.GetComponent<BoxCollider2D>();
            box.size = attackData.hitboxSize;
        }
        else
        {
            CircleCollider2D circle = hitboxObj.GetComponent<CircleCollider2D>();
            circle.radius = attackData.hitboxSize.x;
        }

        AttackHitbox hitbox = hitboxObj.GetComponent<AttackHitbox>();
        float finalDamage = WeaponManager.Instance.CalculateFinalDamage(attackData.damage);
        hitbox.Initialize(finalDamage, attackData.attackName);

        StartCoroutine(ReturnHitboxDelayed(hitboxObj, attackData.hitboxShape, attackData.hitboxDuration));
    }

    void SpawnEffectNow(AttackData attackData)
    {
        if (!attackData.hasEffect || attackData.effectPrefab == null) return;

        float directionMultiplier = transform.localScale.x < 0 ? -1f : 1f;
        Vector2 offset = new Vector2(
            attackData.effectOffset.x * directionMultiplier,
            attackData.effectOffset.y
        );

        Vector2 spawnPos = (Vector2)effectParent.position + offset;

        GameObject effect = Instantiate(attackData.effectPrefab, spawnPos, Quaternion.identity);
        effect.transform.SetParent(effectParent);

        if (transform.localScale.x < 0)
        {
            Vector3 scale = effect.transform.localScale;
            scale.x *= -1;
            effect.transform.localScale = scale;
        }

        Destroy(effect, attackData.effectDuration);
    }

    IEnumerator SpawnHitboxDelayed(AttackData attackData, float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnHitboxNow(attackData);
    }

    IEnumerator SpawnEffectDelayed(AttackData attackData, float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnEffectNow(attackData);
    }

    IEnumerator ReturnHitboxDelayed(GameObject hitbox, HitboxShape shape, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (hitbox != null && HitboxPool.Instance != null)
        {
            HitboxPool.Instance.Return(hitbox, shape);
        }
    }
}