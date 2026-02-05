using UnityEngine;

[System.Serializable]
public class HitboxPhase
{
    [Header("Timing")]
    public float activeTime = 0.1f;
    public float duration = 0.15f;

    [Header("Hitbox")]
    public HitboxShape shape;
    public Vector2 offset;
    public Vector2 size;
    public float radius = 1f;

    [Header("Damage")]
    public float damageMultiplier = 1f;
}

[CreateAssetMenu(fileName = "New Attack", menuName = "Game/Attack Data")]
public class AttackData : ScriptableObject
{
    [Header("Basic Info")]
    public string attackName = "Attack";
    public AttackType attackType;
    public float damage = 10f;
    public float cooldown = 0.5f;

    [Header("Animation")]
    public string animationTrigger = "Attack";
    public float animationDuration = 0.5f;

    [Header("Timing Mode")]
    public TimingMode timingMode = TimingMode.AnimationEvent;

    [Header("Hitbox Settings")]
    public bool useMultiPhaseHitbox = false;

    [Header("Movement Settings")]
    public bool allowMovementDuringAttack = false;

    // 단일 히트박스 (기존)
    public HitboxShape hitboxShape;
    public Vector2 hitboxOffset;
    public Vector2 hitboxSize;
    public float hitboxActiveTime = 0.1f;
    public float hitboxDuration = 0.15f;

    // 다단계 히트박스 (신규)
    public HitboxPhase[] hitboxPhases;

    [Header("Effect (Skill Only)")]
    public bool hasEffect = false;
    public GameObject effectPrefab;
    public Vector2 effectOffset;
    public float effectSpawnTime = 0.1f;
    public float effectDuration = 1f;

    [Header("Dash Skill Settings")]
    public bool isDashSkill = false;
    public float dashDistance = 5f;
    public bool checkObstacles = true;

    [Header("Position Shift (Non-Dash Skills)")]
    public bool shiftsPosition = false;
    public float moveSpeedPerFrame = 0.3f;

    public HitboxPhase GetHitboxPhase(int index)
    {
        if (useMultiPhaseHitbox && hitboxPhases != null && index < hitboxPhases.Length)
        {
            return hitboxPhases[index];
        }

        return new HitboxPhase
        {
            activeTime = hitboxActiveTime,
            duration = hitboxDuration,
            shape = hitboxShape,
            offset = hitboxOffset,
            size = hitboxSize,
            radius = hitboxSize.x,
            damageMultiplier = 1f
        };
    }

    public int GetPhaseCount()
    {
        return useMultiPhaseHitbox && hitboxPhases != null ? hitboxPhases.Length : 1;
    }
}

public enum TimingMode
{
    Time,
    AnimationEvent
}

public enum AttackType
{
    Basic,
    Skill
}

public enum HitboxShape
{
    Box,
    Circle
}
