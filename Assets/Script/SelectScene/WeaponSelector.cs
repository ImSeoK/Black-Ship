using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSelector : MonoBehaviour
{
    [Header("무기 데이터")]
    public WeaponData[] weapons;

    [Header("UI 요소")]
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI descriptionText;
    public Image weaponIcon;
    public Button selectButton;

    [Header("무기 버튼들")]
    public Button spearButton;
    public Button swordButton;
    public Button bowButton;

    private WeaponData currentSelectedWeapon;

    void Start()
    {
        // 버튼 이벤트 등록
        spearButton.onClick.AddListener(() => ShowWeaponInfo(WeaponType.Spear));
        swordButton.onClick.AddListener(() => ShowWeaponInfo(WeaponType.Sword));
        bowButton.onClick.AddListener(() => ShowWeaponInfo(WeaponType.Bow));

        selectButton.onClick.AddListener(ConfirmSelection);

        // 기본 선택 (창)
        ShowWeaponInfo(WeaponType.Spear);
    }

    void ShowWeaponInfo(WeaponType type)
    {
        // 무기 데이터 찾기
        currentSelectedWeapon = System.Array.Find(weapons, w => w.type == type);

        if (currentSelectedWeapon != null)
        {
            // UI 업데이트
            weaponNameText.text = currentSelectedWeapon.weaponName;
            descriptionText.text = currentSelectedWeapon.description;
            weaponIcon.sprite = currentSelectedWeapon.icon;

            // 버튼 하이라이트
            UpdateButtonHighlight(type);
        }
    }

    void UpdateButtonHighlight(WeaponType type)
    {
        // 모든 버튼 기본 색상
        ResetButtonColor(spearButton);
        ResetButtonColor(swordButton);
        ResetButtonColor(bowButton);

        // 선택된 버튼 강조
        Button selectedButton = null;
        switch (type)
        {
            case WeaponType.Spear:
                selectedButton = spearButton;
                break;
            case WeaponType.Sword:
                selectedButton = swordButton;
                break;
            case WeaponType.Bow:
                selectedButton = bowButton;
                break;
        }

        if (selectedButton != null)
        {
            ColorBlock colors = selectedButton.colors;
            colors.normalColor = Color.yellow;
            selectedButton.colors = colors;
        }
    }

    void ResetButtonColor(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        button.colors = colors;
    }

    void ConfirmSelection()
    {
        if (currentSelectedWeapon == null) return;

        // 선택한 무기 저장
        PlayerPrefs.SetInt("SelectedWeapon", (int)currentSelectedWeapon.type);
        PlayerPrefs.Save();

        Debug.Log($"{currentSelectedWeapon.weaponName} 선택!");

        // BaseCamp 씬으로 이동
        if (LoadingManager.Instance != null)
        {
            LoadingManager.Instance.LoadScene("BaseCamp", "DefaultSpawn");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("BaseCamp");
        }
    }
}