using UnityEngine;

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
    public HitboxShape hitboxShape;
    public Vector2 hitboxOffset;
    public Vector2 hitboxSize;
    public float hitboxActiveTime = 0.1f;
    public float hitboxDuration = 0.15f;

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