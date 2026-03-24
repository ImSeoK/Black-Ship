using UnityEngine;
using System.Collections;

/// <summary>
/// 플레이어 공격 히트박스 생성/반환을 담당합니다.
/// AttackController와 같은 GameObject에 부착되어야 합니다.
/// hitboxParent는 AttackController.Start()에서 자동으로 설정됩니다.
/// </summary>
public class HitboxSpawner : MonoBehaviour
{
    [HideInInspector] public Transform hitboxParent;

    public void SpawnHitbox(AttackData attackData, float directionMultiplier)
    {
#if UNITY_EDITOR
        Debug.Log("=== SpawnHitboxNow called ===");
#endif
        if (HitboxPool.Instance == null)
        {
#if UNITY_EDITOR
            Debug.LogError("HitboxPool not found!");
#endif
            return;
        }

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
#if UNITY_EDITOR
            Debug.Log("Hitbox Box created at " + spawnPos + ", size: " + attackData.hitboxSize +
                      "\nLayer: " + LayerMask.LayerToName(hitboxObj.layer) +
                      "\nIs Trigger: " + box.isTrigger);
#endif
        }
        else
        {
            CircleCollider2D circle = hitboxObj.GetComponent<CircleCollider2D>();
            circle.radius = attackData.hitboxSize.x;
#if UNITY_EDITOR
            Debug.Log("Hitbox Circle created at " + spawnPos + ", radius: " + attackData.hitboxSize.x +
                      "\nLayer: " + LayerMask.LayerToName(hitboxObj.layer) +
                      "\nIs Trigger: " + circle.isTrigger);
#endif
        }

        float finalDamage = WeaponManager.Instance.CalculateFinalDamage(attackData.damage);
        AttackHitbox hitbox = hitboxObj.GetComponent<AttackHitbox>();
        hitbox.Initialize(finalDamage, attackData.attackName);

#if UNITY_EDITOR
        Debug.Log("Hitbox initialized with damage: " + finalDamage);
#endif

        StartCoroutine(ReturnHitboxDelayed(hitboxObj, attackData.hitboxShape, attackData.hitboxDuration));
    }

    public void SpawnHitboxPhase(AttackData attackData, int phaseIndex, float directionMultiplier)
    {
#if UNITY_EDITOR
        Debug.Log($"=== SpawnHitboxPhase {phaseIndex} ===");
#endif
        if (HitboxPool.Instance == null)
        {
#if UNITY_EDITOR
            Debug.LogError("HitboxPool not found!");
#endif
            return;
        }

        HitboxPhase phase = attackData.GetHitboxPhase(phaseIndex);

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
#if UNITY_EDITOR
            Debug.Log($"Phase {phaseIndex} Box at {spawnPos}, size: {phase.size}");
#endif
        }
        else
        {
            CircleCollider2D circle = hitboxObj.GetComponent<CircleCollider2D>();
            circle.radius = phase.radius;
            circle.offset = Vector2.zero;
#if UNITY_EDITOR
            Debug.Log($"Phase {phaseIndex} Circle at {spawnPos}, radius: {phase.radius}");
#endif
        }

        float phaseDamage = attackData.damage * phase.damageMultiplier;
        float finalDamage = WeaponManager.Instance.CalculateFinalDamage(phaseDamage);
        AttackHitbox hitbox = hitboxObj.GetComponent<AttackHitbox>();
        hitbox.Initialize(finalDamage, $"{attackData.attackName}_Phase{phaseIndex}");

#if UNITY_EDITOR
        Debug.Log($"Phase {phaseIndex} damage: {finalDamage}");
#endif

        StartCoroutine(ReturnHitboxDelayed(hitboxObj, phase.shape, phase.duration));
    }

    public void CreatePathHitbox(Vector2 startPos, Vector2 endPos, float duration, AttackData attackData)
    {
        Vector2 center = (startPos + endPos) / 2f;
        float distance = Vector2.Distance(startPos, endPos);

        GameObject hitboxObj = new GameObject(attackData.attackName + "_PathHitbox");
        hitboxObj.transform.position = center;
        hitboxObj.transform.SetParent(hitboxParent);
        hitboxObj.layer = LayerMask.NameToLayer("PlayerAttack");

        BoxCollider2D box = hitboxObj.AddComponent<BoxCollider2D>();
        box.size = new Vector2(distance, 1f);
        box.isTrigger = true;

        float angle = Mathf.Atan2(endPos.y - startPos.y, endPos.x - startPos.x) * Mathf.Rad2Deg;
        hitboxObj.transform.rotation = Quaternion.Euler(0, 0, angle);

        AttackHitbox hitbox = hitboxObj.AddComponent<AttackHitbox>();
        float finalDamage = WeaponManager.Instance.CalculateFinalDamage(attackData.damage);
        hitbox.Initialize(finalDamage, attackData.attackName);

        Destroy(hitboxObj, duration);
    }

    IEnumerator ReturnHitboxDelayed(GameObject hitbox, HitboxShape shape, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (hitbox != null && HitboxPool.Instance != null)
            HitboxPool.Instance.Return(hitbox, shape);
    }
}
