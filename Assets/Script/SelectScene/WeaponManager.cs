using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    [Header("현재 무기")]
    public WeaponType currentWeapon = WeaponType.None;
    public WeaponData currentWeaponData;
    public WeaponAttackSet currentAttackSet; // 추가!

    [Header("무기 데이터베이스")]
    public WeaponData[] allWeapons;

    [Header("공격 세트 데이터베이스")]
    public WeaponAttackSet[] allAttackSets; // 추가!

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

    public void EquipWeapon(WeaponType weapon)
    {
        currentWeapon = weapon;

        // 무기 데이터 찾기
        currentWeaponData = System.Array.Find(allWeapons, w => w.weaponType == weapon);

        // 공격 세트 찾기
        currentAttackSet = System.Array.Find(allAttackSets, s => s.weaponType == weapon);

        if (currentWeaponData != null)
        {
            Debug.Log($"무기 장착: {currentWeaponData.weaponName}");

            if (currentAttackSet == null)
            {
                Debug.LogWarning($"{weapon}의 WeaponAttackSet이 없습니다!");
            }
        }
        else
        {
            Debug.LogError($"{weapon} 무기 데이터를 찾을 수 없습니다!");
        }
    }

    public bool CanAttack()
    {
        return currentWeapon != WeaponType.None && currentAttackSet != null;
    }

    // 최종 데미지 계산 (무기 배율 + 크리티컬)
    public float CalculateFinalDamage(float baseDamage)
    {
        if (currentWeaponData == null) return baseDamage;

        float damage = baseDamage * currentWeaponData.damageMultiplier;

        // 크리티컬 계산
        if (currentWeaponData.hasCritical)
        {
            float roll = Random.Range(0f, 100f);
            if (roll < currentWeaponData.criticalChance)
            {
                damage *= currentWeaponData.criticalMultiplier;
                Debug.Log($" 크리티컬! ({currentWeaponData.criticalMultiplier}x)");
            }
        }

        return damage;
    }

    // 최종 쿨다운 계산 (무기 공격속도 배율)
    public float CalculateFinalCooldown(float baseCooldown)
    {
        if (currentWeaponData == null) return baseCooldown;

        return baseCooldown / currentWeaponData.attackSpeedMultiplier;
    }
}