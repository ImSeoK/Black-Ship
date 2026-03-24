using UnityEngine;

/// <summary>
/// 플레이어 공격 이펙트 생성을 담당합니다.
/// AttackController와 같은 GameObject에 부착되어야 합니다.
/// effectParent는 AttackController.Start()에서 자동으로 설정됩니다.
/// </summary>
public class EffectSpawner : MonoBehaviour
{
    [HideInInspector] public Transform effectParent;

    public void SpawnEffect(AttackData attackData, float directionMultiplier)
    {
        if (!attackData.hasEffect || attackData.effectPrefab == null) return;

        Vector2 offset = new Vector2(
            attackData.effectOffset.x * directionMultiplier,
            attackData.effectOffset.y
        );
        Vector2 spawnPos = (Vector2)effectParent.position + offset;

        GameObject effect = Instantiate(attackData.effectPrefab, spawnPos, Quaternion.identity);
        effect.transform.SetParent(effectParent);

        if (directionMultiplier < 0)
        {
            Vector3 scale = effect.transform.localScale;
            scale.x *= -1;
            effect.transform.localScale = scale;
        }

        Destroy(effect, attackData.effectDuration);
    }
}
