using UnityEngine;

public class BabyCarryController : MonoBehaviour
{
    [Header("아기 스프라이트")]
    public GameObject babySprite;

    [Header("키 설정")]
    public KeyCode putDownKey = KeyCode.R;

    void Update()
    {
        // 유아기일 때만 스프라이트 표시
        if (babySprite != null)
        {
            babySprite.SetActive(
                BabyManager.Instance != null &&
                BabyManager.Instance.carryingBaby &&
                BabyManager.Instance.IsInfant()
            );
        }

        if (BabyManager.Instance == null || !BabyManager.Instance.carryingBaby) return;

        if (Input.GetKeyDown(putDownKey))
        {
            if (BabyManager.Instance.IsInfant())
            {
                // 유아기 - 그냥 내려놓기
                PutDownBaby();
            }
            else
            {
                // 이후 시기 - 두고가기 선택지
                ShowLeaveChoiceUI();
            }
        }
    }

    void PutDownBaby()
    {
        Vector3 dropPosition = transform.position;
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        BabyManager.Instance.PutDownBaby(dropPosition, currentScene);
        ActivateBabyOnGround();

        Debug.Log($"아기를 {currentScene}에 내려놓았습니다!");
    }

    void ShowLeaveChoiceUI()
    {
        // 유아기 이후 - 데려가기/두고가기 선택
        if (ChoiceUI.Instance != null)
        {
            ChoiceUI.Instance.ShowChoice(
                transform,
                "두고가기",
                "취소",
                OnLeave,
                OnCancel
            );
        }
    }

    void OnLeave()
    {
        // 두고가기 선택 시 - 현재 씬에 배치
        Vector3 dropPosition = transform.position;
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        BabyManager.Instance.PutDownBaby(dropPosition, currentScene);
        ActivateBabyOnGround();

        Debug.Log($"아기를 {currentScene}에 두고 갑니다.");
    }

    void OnCancel()
    {
        Debug.Log("취소했습니다.");
    }

    void ActivateBabyOnGround()
    {
        BabyOnGround babyOnGround = FindFirstObjectByType<BabyOnGround>(FindObjectsInactive.Include);

        if (babyOnGround != null)
        {
            babyOnGround.gameObject.SetActive(true);
            babyOnGround.transform.position = BabyManager.Instance.babyPosition;
        }
    }
}