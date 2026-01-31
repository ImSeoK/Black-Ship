using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("기본 정보")]
    public WeaponType weaponType;
    public string weaponName;
    [TextArea(3, 5)]
    public string description;
    public Sprite icon;

    [Header("스탯 배율")] // 수정: 절대값 → 배율
    [Range(0.5f, 3f)]
    public float damageMultiplier = 1f;
    [Range(0.5f, 2f)]
    public float attackSpeedMultiplier = 1f;
    [Range(0.5f, 2f)]
    public float rangeMultiplier = 1f;

    [Header("애니메이션")]
    public RuntimeAnimatorController animatorController;

    [Header("추가 효과")]
    public bool hasSpecialAbility;
    [TextArea(2, 3)]
    public string specialAbilityDescription;

    [Header("크리티컬 (옵션)")]
    public bool hasCritical = false;
    [Range(0f, 100f)]
    public float criticalChance = 10f;
    [Range(1f, 3f)]
    public float criticalMultiplier = 1.5f;
}