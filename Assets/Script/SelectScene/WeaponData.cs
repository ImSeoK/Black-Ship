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

    [Header("전투 스탯")]
    public int damage;
    public float attackCooldown;
    public float range;
    public float knockback;

    [Header("애니메이션")]
    public RuntimeAnimatorController animatorController;

    [Header("추가 효과")]
    public bool hasSpecialAbility;
    [TextArea(2, 3)]
    public string specialAbilityDescription;
}