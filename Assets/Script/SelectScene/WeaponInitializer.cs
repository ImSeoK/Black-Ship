using UnityEngine;

public class WeaponInitializer : MonoBehaviour
{
    [Header("애니메이터 컨트롤러")]
    public RuntimeAnimatorController spearAnimator;
    public RuntimeAnimatorController swordAnimator;
    public RuntimeAnimatorController bowAnimator;

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

        // Animator 교체
        ChangeAnimator(weaponType);

        // WeaponManager에 설정
        WeaponManager weaponManager = GetComponent<WeaponManager>();
        if (weaponManager != null)
        {
            weaponManager.EquipWeapon(weaponType);
        }
    }

    void ChangeAnimator(WeaponType type)
    {
        RuntimeAnimatorController controller = null;

        switch (type)
        {
            case WeaponType.Spear:
                controller = spearAnimator;
                break;
            case WeaponType.Sword:
                controller = swordAnimator;
                break;
            case WeaponType.Bow:
                controller = bowAnimator;
                break;
        }

        if (controller != null && animator != null)
        {
            animator.runtimeAnimatorController = controller;
        }
    }
}