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

            yield return new WaitForSeconds(attackData.animationDuration);
        }

        isAttacking = false;
        currentAttack = null;
    }

    IEnumerator ExecuteDash(AttackData attackData)
    {
        float direction = spriteRenderer.flipX ? -1f : 1f;
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
                Debug.Log("Obstacle detected! Distance adjusted: " + attackData.dashDistance + " -> " + safeDistance);
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
            yield return null;
        }

        transform.position = endPos;
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

        Debug.Log("Path hitbox created: " + startPos + " -> " + endPos);

        Destroy(hitboxObj, duration);
    }

    public void OnAttackHitboxSpawn()
    {
        if (currentAttack == null) return;
        Debug.Log("[Animation Event] Hitbox spawned!");
        SpawnHitboxNow(currentAttack);
    }

    public void OnAttackEffectSpawn()
    {
        if (currentAttack == null) return;
        Debug.Log("[Animation Event] Effect spawned!");
        SpawnEffectNow(currentAttack);
    }

    public void OnAttackEnd()
    {
        Debug.Log("[Animation Event] Attack ended!");
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

        float directionMultiplier = spriteRenderer.flipX ? -1f : 1f;
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

        Debug.Log("Hitbox created: " + spawnPos + ", Damage: " + finalDamage);

        StartCoroutine(ReturnHitboxDelayed(hitboxObj, attackData.hitboxShape, attackData.hitboxDuration));
    }

    void SpawnEffectNow(AttackData attackData)
    {
        if (!attackData.hasEffect || attackData.effectPrefab == null) return;

        float directionMultiplier = spriteRenderer.flipX ? -1f : 1f;
        Vector2 offset = new Vector2(
            attackData.effectOffset.x * directionMultiplier,
            attackData.effectOffset.y
        );

        Vector2 spawnPos = (Vector2)effectParent.position + offset;

        GameObject effect = Instantiate(attackData.effectPrefab, spawnPos, Quaternion.identity);
        effect.transform.SetParent(effectParent);

        if (spriteRenderer.flipX)
        {
            Vector3 scale = effect.transform.localScale;
            scale.x *= -1;
            effect.transform.localScale = scale;
        }

        Debug.Log("Effect created: " + attackData.effectPrefab.name);

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