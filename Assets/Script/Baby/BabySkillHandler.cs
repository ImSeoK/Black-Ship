using UnityEngine;

public class BabySkillHandler : MonoBehaviour
{
    public static BabySkillHandler Instance;

    public enum Personality
    {
        Aggressive,  // 적극적 (호감도 70~100)
        Neutral,     // 중립   (호감도 30~69)
        Passive,     // 소극적 (호감도 0~29)
    }

    [Header("현재 성격")]
    public Personality currentPersonality = Personality.Neutral;

    [Header("패시브 수치 (Inspector에서 조정)")]
    public float aggressiveAttackBonus = 1.1f;   // 공격력 10% 증가
    public float neutralDefenseBonus = 0.9f;     // 피해량 10% 감소
    public float passiveSpeedBonus = 1.15f;      // 이동속도 15% 증가

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

    void Start()
    {
        UpdatePersonality();
    }

    // 호감도 기반으로 성격 결정
    public void UpdatePersonality()
    {
        if (BabyManager.Instance == null) return;

        int affection = BabyManager.Instance.babyStats.affection;

        if (affection >= 70)
            currentPersonality = Personality.Aggressive;
        else if (affection >= 30)
            currentPersonality = Personality.Neutral;
        else
            currentPersonality = Personality.Passive;

        Debug.Log($"성격 업데이트: {currentPersonality} (호감도: {affection})");

        // 패시브 적용
        ApplyPassive();
    }

    void ApplyPassive()
    {
        switch (currentPersonality)
        {
            case Personality.Aggressive:
                ApplyAggressivePassive();
                break;
            case Personality.Neutral:
                ApplyNeutralPassive();
                break;
            case Personality.Passive:
                ApplyPassivePassive();
                break;
        }
    }

    // ─── 패시브 효과 ─────────────────────────────────────
    // 지금은 더미. 나중에 StatsManager 연동해서 실제 수치 반영

    void ApplyAggressivePassive()
    {
        // TODO: 공격력 aggressiveAttackBonus 배율 적용
        // 예: StatsManager.Instance.attackMultiplier = aggressiveAttackBonus;
        Debug.Log($"[적극적] 공격력 {(aggressiveAttackBonus - 1) * 100}% 증가");
    }

    void ApplyNeutralPassive()
    {
        // TODO: 피해량 감소 적용
        // 예: StatsManager.Instance.damageReduction = neutralDefenseBonus;
        Debug.Log($"[중립] 피해량 {(1 - neutralDefenseBonus) * 100}% 감소");
    }

    void ApplyPassivePassive()
    {
        // TODO: 이동속도 증가 PlayerMovement 연동
        // 예: PlayerMovement.Instance.speedMultiplier = passiveSpeedBonus;
        Debug.Log($"[소극적] 이동속도 {(passiveSpeedBonus - 1) * 100}% 증가");
    }

    // ─── 외부 호출용 ─────────────────────────────────────

    // 공격력 배율 반환 (AttackController에서 사용)
    public float GetAttackMultiplier()
    {
        return currentPersonality == Personality.Aggressive ? aggressiveAttackBonus : 1f;
    }

    // 피해량 배율 반환 (StatsManager TakeDamage에서 사용)
    public float GetDamageMultiplier()
    {
        return currentPersonality == Personality.Neutral ? neutralDefenseBonus : 1f;
    }

    // 이동속도 배율 반환 (PlayerMovement에서 사용)
    public float GetSpeedMultiplier()
    {
        return currentPersonality == Personality.Passive ? passiveSpeedBonus : 1f;
    }
}