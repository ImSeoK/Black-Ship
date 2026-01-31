using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Attack Set", menuName = "Game/Weapon Attack Set")]
public class WeaponAttackSet : ScriptableObject
{
    [Header("무기 타입")]
    public WeaponType weaponType;

    [Header("공격 패턴")]
    public AttackData[] attacks; // [0]: 기본공격, [1]: 스킬1, [2]: 스킬2...

    // 편의 함수
    public AttackData GetBasicAttack()
    {
        return attacks != null && attacks.Length > 0 ? attacks[0] : null;
    }

    public AttackData GetSkill(int skillIndex)
    {
        int index = skillIndex + 1; // 0: 스킬1, 1: 스킬2
        return attacks != null && attacks.Length > index ? attacks[index] : null;
    }
}