using UnityEngine;

public class WeaponInitializer : MonoBehaviour
{
    [Header("무기 데이터베이스")]
    public WeaponData[] weaponDatabase;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        // 저장된 무기 불러오기
        int savedWeapon = PlayerPrefs.GetInt("SelectedWeapon", 0);
        WeaponType weaponType = (WeaponType)savedWeapon;

        if (weaponType == WeaponType.None)
        {
            Debug.LogWarning("선택된 무기가 없습니다!");
            return;
        }

        // 무기 데이터 찾기
        WeaponData weaponData = System.Array.Find(weaponDatabase, w => w.weaponType == weaponType);

        if (weaponData != null)
        {
            // Animator 교체
            if (weaponData.animatorController != null)
            {
                animator.runtimeAnimatorController = weaponData.animatorController;
            }

            // WeaponManager에 설정
            WeaponManager weaponManager = GetComponent<WeaponManager>();
            if (weaponManager != null)
            {
                weaponManager.EquipWeapon(weaponType);
            }
        }
    }
}