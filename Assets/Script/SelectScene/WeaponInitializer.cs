using UnityEngine;

public class WeaponInitializer : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        int savedWeapon = PlayerPrefs.GetInt("SelectedWeapon", 0);
        WeaponType weaponType = (WeaponType)savedWeapon;

        if (weaponType == WeaponType.None)
        {
#if UNITY_EDITOR
            Debug.LogWarning("선택된 무기가 없습니다!");
#endif
            return;
        }

        if (WeaponManager.Instance == null)
        {
#if UNITY_EDITOR
            Debug.LogError("WeaponManager not found!");
#endif
            return;
        }

        // WeaponManager가 단일 소스 — 여기서 장착하면 currentWeaponData/currentAttackSet 모두 설정됨
        WeaponManager.Instance.EquipWeapon(weaponType);

        // EquipWeapon 이후 currentWeaponData에서 Animator 교체
        RuntimeAnimatorController controller = WeaponManager.Instance.currentWeaponData?.animatorController;
        if (controller != null)
            animator.runtimeAnimatorController = controller;
    }
}
