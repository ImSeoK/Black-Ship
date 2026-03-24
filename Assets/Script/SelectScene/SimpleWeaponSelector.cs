using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleWeaponSelector : MonoBehaviour
{
    [Header("UI")]
    public Button[] weaponButtons;
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI descriptionText;

    private WeaponData[] weaponList;
    private WeaponData selectedWeapon;

    void Start()
    {
        // WeaponManager를 단일 소스로 사용
        if (WeaponManager.Instance == null || WeaponManager.Instance.allWeapons == null)
        {
#if UNITY_EDITOR
            Debug.LogError("WeaponManager 또는 allWeapons가 없습니다!");
#endif
            return;
        }

        weaponList = WeaponManager.Instance.allWeapons;

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
            LoadingManager.Instance.LoadScene("BaseCamp", "DefaultSpawn");
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("BaseCamp");
    }
}
