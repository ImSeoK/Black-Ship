using UnityEngine;

public class WeaponInteraction : MonoBehaviour
{
    [Header("무기 설정")]
    public WeaponData weaponData; // 이 오브젝트의 무기

    private InteractableChoice interactable;

    void Start()
    {
        interactable = GetComponent<InteractableChoice>();

        if (interactable != null)
        {
            // Yes 선택 시 무기 장착
            interactable.onYesSelected.AddListener(OnWeaponEquip);
        }
    }

    void OnWeaponEquip()
    {
        if (weaponData == null) return;

        Debug.Log($"{weaponData.weaponName} 장착!");

        // WeaponManager에 장착
        if (WeaponManager.Instance != null)
        {
            WeaponManager.Instance.EquipWeapon(weaponData.weaponType);
        }

        // 오브젝트 비활성화 (선택사항)
        // gameObject.SetActive(false);
    }
}