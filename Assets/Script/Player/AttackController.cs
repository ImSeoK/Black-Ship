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

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (hitboxParent == null) hitboxParent = transform;
        if (effectParent == null) effectParent = transform;

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

            if (attackData.shiftsPosition)
            {
                // 매 프레임 이동 방식
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
        float movePerFrame = attackData.moveSpeedPerFrame * direction;
        float duration = attackData.animationDuration;
        float elapsed = 0f;
        Vector2 startPos = transform.position;
        int frameCount = 0;

        // 첫 프레임 대기 (애니메이션 시작 대기)
        yield return null;
        frameCount++;
        elapsed += Time.deltaTime;

        while (elapsed < duration)
        {
            // 매 프레임 일정하게 이동
            Vector2 movement = new Vector2(movePerFrame, 0);
            transform.position = (Vector2)transform.position + movement;

            frameCount++;

            // 5프레임마다 로그
            if (frameCount % 5 == 0)
            {
                float movedDistance = ((Vector2)transform.position - startPos).x;
                Debug.Log($"Frame {frameCount}: Elapsed {elapsed:F2}s, Moved {movedDistance:F2}");
            }

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            yield return null;
            elapsed += Time.deltaTime;
        }

        Vector2 endPos = transform.position;
        float totalMoved = (endPos - startPos).x;
        Debug.Log($"=== Skill End === Total Frames: {frameCount}, Total Distance: {totalMoved:F2}");
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

    // ===== 다단계 히트박스 지원 메서드 =====

    public void OnAttackHitboxSpawn()
    {
        if (currentAttack == null) return;

        if (currentAttack.useMultiPhaseHitbox)
        {
            Debug.LogWarning("Multi-phase hitbox should use SpawnHitboxPhase(index) instead!");
        }
        else
        {
            SpawnHitboxNow(currentAttack);
        }
    }

    // Animation Event에서 호출: SpawnHitboxPhase(0), SpawnHitboxPhase(1) 등
    public void SpawnHitboxPhase(int phaseIndex)
    {
        if (currentAttack == null)
        {
            Debug.LogError("No current attack!");
            return;
        }

        if (!currentAttack.useMultiPhaseHitbox)
        {
            Debug.LogWarning("Attack is not multi-phase! Use OnAttackHitboxSpawn() instead.");
            return;
        }

        if (phaseIndex >= currentAttack.GetPhaseCount())
        {
            Debug.LogError($"Phase index {phaseIndex} out of range!");
            return;
        }

        SpawnHitboxPhaseNow(currentAttack, phaseIndex);
    }

    void SpawnHitboxPhaseNow(AttackData attackData, int phaseIndex)
    {
        Debug.Log($"=== SpawnHitboxPhase {phaseIndex} ===");

        if (HitboxPool.Instance == null)
        {
            Debug.LogError("HitboxPool not found!");
            return;
        }

        HitboxPhase phase = attackData.GetHitboxPhase(phaseIndex);
        float directionMultiplier = transform.localScale.x < 0 ? -1f : 1f;

        Vector2 offset = new Vector2(
            phase.offset.x * directionMultiplier,
            phase.offset.y
        );
        Vector2 spawnPos = (Vector2)hitboxParent.position + offset;

        GameObject hitboxObj = HitboxPool.Instance.Get(phase.shape);
        hitboxObj.transform.position = spawnPos;
        hitboxObj.transform.SetParent(hitboxParent);

        if (phase.shape == HitboxShape.Box)
        {
            BoxCollider2D box = hitboxObj.GetComponent<BoxCollider2D>();
            box.size = phase.size;
            box.offset = Vector2.zero;

            Debug.Log($"Phase {phaseIndex} Box at {spawnPos}, size: {phase.size}");
        }
        else
        {
            CircleCollider2D circle = hitboxObj.GetComponent<CircleCollider2D>();
            circle.radius = phase.radius;
            circle.offset = Vector2.zero;

            Debug.Log($"Phase {phaseIndex} Circle at {spawnPos}, radius: {phase.radius}");
        }

        float phaseDamage = attackData.damage * phase.damageMultiplier;
        float finalDamage = WeaponManager.Instance.CalculateFinalDamage(phaseDamage);

        AttackHitbox hitbox = hitboxObj.GetComponent<AttackHitbox>();
        hitbox.Initialize(finalDamage, $"{attackData.attackName}_Phase{phaseIndex}");

        Debug.Log($"Phase {phaseIndex} damage: {finalDamage}");

        StartCoroutine(ReturnHitboxDelayed(hitboxObj, phase.shape, phase.duration));
    }

    IEnumerator SpawnHitboxPhaseDelayed(AttackData attackData, int phaseIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnHitboxPhaseNow(attackData, phaseIndex);
    }

    // ===== 기존 메서드들 =====

    public void OnAttackEffectSpawn()
    {
        if (currentAttack == null) return;
        SpawnEffectNow(currentAttack);
    }

    public void OnAttackEnd()
    {
        isAttacking = false;
        currentAttack = null;
    }

    void SpawnHitboxNow(AttackData attackData)
    {
        Debug.Log("=== SpawnHitboxNow called ===");

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

            Debug.Log("Hitbox Box created at " + spawnPos + ", size: " + attackData.hitboxSize +
                      "\nLayer: " + LayerMask.LayerToName(hitboxObj.layer) +
                      "\nIs Trigger: " + box.isTrigger);
        }
        else
        {
            CircleCollider2D circle = hitboxObj.GetComponent<CircleCollider2D>();
            circle.radius = attackData.hitboxSize.x;

            Debug.Log("Hitbox Circle created at " + spawnPos + ", radius: " + attackData.hitboxSize.x +
                      "\nLayer: " + LayerMask.LayerToName(hitboxObj.layer) +
                      "\nIs Trigger: " + circle.isTrigger);
        }

        AttackHitbox hitbox = hitboxObj.GetComponent<AttackHitbox>();
        float finalDamage = WeaponManager.Instance.CalculateFinalDamage(attackData.damage);
        hitbox.Initialize(finalDamage, attackData.attackName);

        Debug.Log("Hitbox initialized with damage: " + finalDamage);

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

    void OnDrawGizmosSelected()
    {
        if (attacks != null && attacks.Count > 1)
        {
            AttackData previewAttack = attacks[1];

            float directionMultiplier = transform.localScale.x < 0 ? -1f : 1f;

            // 다단계 히트박스 미리보기
            if (previewAttack.useMultiPhaseHitbox && previewAttack.hitboxPhases != null)
            {
                Color[] phaseColors = new Color[]
                {
                    new Color(0f, 1f, 0f, 0.3f),    // 초록
                    new Color(1f, 0.5f, 0f, 0.3f),  // 주황
                    new Color(1f, 0f, 1f, 0.3f)     // 보라
                };

                for (int i = 0; i < previewAttack.hitboxPhases.Length; i++)
                {
                    HitboxPhase phase = previewAttack.hitboxPhases[i];

                    Vector2 offset = new Vector2(
                        phase.offset.x * directionMultiplier,
                        phase.offset.y
                    );
                    Vector2 hitboxPos = (Vector2)transform.position + offset;

                    Gizmos.color = phaseColors[i % phaseColors.Length];

                    if (phase.shape == HitboxShape.Box)
                    {
                        Gizmos.DrawCube(hitboxPos, phase.size);
                        Gizmos.color = Color.Lerp(phaseColors[i % phaseColors.Length], Color.white, 0.5f);
                        Gizmos.DrawWireCube(hitboxPos, phase.size);
                    }
                    else
                    {
                        Gizmos.DrawSphere(hitboxPos, phase.radius);
                        Gizmos.color = Color.Lerp(phaseColors[i % phaseColors.Length], Color.white, 0.5f);
                        Gizmos.DrawWireSphere(hitboxPos, phase.radius);
                    }
                }
            }
            else
            {
                // 기존 단일 히트박스 미리보기
                Vector2 offset = new Vector2(
                    previewAttack.hitboxOffset.x * directionMultiplier,
                    previewAttack.hitboxOffset.y
                );

                Vector2 hitboxPos = (Vector2)transform.position + offset;

                Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
                if (previewAttack.hitboxShape == HitboxShape.Box)
                {
                    Gizmos.DrawCube(hitboxPos, previewAttack.hitboxSize);
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(hitboxPos, previewAttack.hitboxSize);
                }
                else
                {
                    Gizmos.DrawSphere(hitboxPos, previewAttack.hitboxSize.x);
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(hitboxPos, previewAttack.hitboxSize.x);
                }
            }

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.2f);
        }
    }
}
