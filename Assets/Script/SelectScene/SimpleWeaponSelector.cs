using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleWeaponSelector : MonoBehaviour
{
    [Header("UI")]
    public Button[] weaponButtons;
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI descriptionText;

    [Header("무기 데이터 (WeaponManager가 없을 때 사용)")]
    public WeaponData[] fallbackWeapons;

    private WeaponData[] weaponList;
    private WeaponData selectedWeapon;

    void Start()
    {
        // WeaponManager가 있으면 우선 사용, 없으면 로컬 fallback 사용
        if (WeaponManager.Instance != null && WeaponManager.Instance.allWeapons != null && WeaponManager.Instance.allWeapons.Length > 0)
        {
            weaponList = WeaponManager.Instance.allWeapons;
        }
        else if (fallbackWeapons != null && fallbackWeapons.Length > 0)
        {
            weaponList = fallbackWeapons;
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogError("WeaponManager 또는 fallbackWeapons가 없습니다!");
#endif
            return;
        }

        for (int i = 0; i < weaponButtons.Length; i++)
        {
            if (i < weaponList.Length)
            {
                int index = i;
                weaponButtons[i].onClick.AddListener(() => OnWeaponButtonClick(index));
            }
        }

        if (weaponList.Length > 0)
            ShowWeaponInfo(0);
    }

    void OnWeaponButtonClick(int index)
    {
        ShowWeaponInfo(index);
        ConfirmSelection();
    }

    void ShowWeaponInfo(int index)
    {
        selectedWeapon = weaponList[index];

        if (selectedWeapon != null)
        {
            if (weaponNameText != null)
                weaponNameText.text = selectedWeapon.weaponName;

            if (descriptionText != null)
                descriptionText.text = selectedWeapon.description;
        }
    }

    void ConfirmSelection()
    {
        if (selectedWeapon == null) return;

        if (CursorManager.Instance != null)
            CursorManager.Instance.SetWeaponCursor(selectedWeapon.weaponType);

        PlayerPrefs.SetInt("SelectedWeapon", (int)selectedWeapon.weaponType);
        PlayerPrefs.Save();

        if (LoadingManager.Instance != null)
            LoadingManager.Instance.LoadScene("Prologue", "");
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("Prologue");
    }
}
