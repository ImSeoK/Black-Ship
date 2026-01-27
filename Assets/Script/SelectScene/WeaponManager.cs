using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    [Header("현재 무기")]
    public WeaponType currentWeapon = WeaponType.None;
    public WeaponData currentWeaponData; // 추가!

    [Header("무기 데이터베이스")]
    public WeaponData[] allWeapons; // 모든 무기 데이터

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

        if (currentWeaponData != null)
        {
            Debug.Log($"무기 장착: {currentWeaponData.weaponName} (데미지: {currentWeaponData.damage})");
        }
    }

    public bool CanAttack()
    {
        return currentWeapon != WeaponType.None;
    }
}