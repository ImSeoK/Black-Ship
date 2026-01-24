using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    [Header("현재 무기")]
    public WeaponType currentWeapon = WeaponType.None;

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
    }

    public bool CanAttack()
    {
        return currentWeapon != WeaponType.None;
    }
}