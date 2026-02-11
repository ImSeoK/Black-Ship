using UnityEngine;

public class BabyOnGround : MonoBehaviour
{
    [Header("상호작용 설정")]
    public KeyCode interactKey = KeyCode.E;

    private bool playerNearby = false;

    void Update()
    {
        // babyPickedUp이고 carryingBaby가 false일 때만 활성화
        bool shouldBeActive = StatsManager.Instance.babyPickedUp &&
                             !StatsManager.Instance.carryingBaby &&
                             StatsManager.Instance.babySceneName == UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        gameObject.SetActive(shouldBeActive);

        if (shouldBeActive)
        {
            transform.position = StatsManager.Instance.babyPosition;
        }

        // E키 입력 감지
        if (playerNearby && Input.GetKeyDown(interactKey))
        {
            ShowChoiceUI();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            // 상호작용 UI 표시 (옵션)
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            // 상호작용 UI 숨김 (옵션)
        }
    }

    void ShowChoiceUI()
    {
        if (ChoiceUI.Instance != null)
        {
            ChoiceUI.Instance.ShowChoice(
                transform,
                "들기", // Yes 텍스트
                "취소", // No 텍스트
                OnPickUp, // Yes 액션
                OnCancel  // No 액션
            );
        }
    }

    void OnPickUp()
    {
        Debug.Log("아이를 들었습니다!");

        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.carryingBaby = true;
            StatsManager.Instance.SaveState();
        }

        gameObject.SetActive(false);
    }

    void OnCancel()
    {
        Debug.Log("취소했습니다.");
        // 아무것도 안 함
    }
}