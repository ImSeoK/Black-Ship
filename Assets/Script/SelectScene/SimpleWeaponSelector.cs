using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleWeaponSelector : MonoBehaviour
{
    [Header("무기 데이터")]
    public WeaponData[] weaponDataList;

    [Header("UI")]
    public Button[] weaponButtons; // 모든 무기 버튼 배열
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI descriptionText;

    private WeaponData selectedWeapon;

    void Start()
    {
        // 각 버튼에 이벤트 등록
        for (int i = 0; i < weaponButtons.Length; i++)
        {
            if (i < weaponDataList.Length)
            {
                int index = i; // 클로저 문제 해결
                weaponButtons[i].onClick.AddListener(() => OnWeaponButtonClick(index));
            }
        }

        // 기본 선택 (첫 번째 무기)
        if (weaponDataList.Length > 0)
        {
            ShowWeaponInfo(0);
        }
    }

    void OnWeaponButtonClick(int index)
    {
        // 무기 정보 표시
        ShowWeaponInfo(index);

        // 선택 완료
        ConfirmSelection();
    }

    void ShowWeaponInfo(int index)
    {
        selectedWeapon = weaponDataList[index];

        if (selectedWeapon != null)
        {
            // UI 업데이트
            if (weaponNameText != null)
                weaponNameText.text = selectedWeapon.weaponName;

            if (descriptionText != null)
                descriptionText.text = selectedWeapon.description;

            Debug.Log($"{selectedWeapon.weaponName} 정보 표시");
        }
    }

    void ConfirmSelection()
    {
        if (selectedWeapon == null) return;

        Debug.Log($"{selectedWeapon.weaponName} 선택!");

        // 커서 미리보기
        if (CursorManager.Instance != null)
        {
            CursorManager.Instance.SetWeaponCursor(selectedWeapon.weaponType);
        }

        // PlayerPrefs에 저장
        PlayerPrefs.SetInt("SelectedWeapon", (int)selectedWeapon.weaponType);
        PlayerPrefs.Save();

        // BaseCamp로 이동
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