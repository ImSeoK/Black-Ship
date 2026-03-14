using UnityEngine;

public class BabyOnGround : MonoBehaviour
{
    [Header("상호작용 설정")]
    public KeyCode interactKey = KeyCode.E;

    private bool playerNearby = false;

    void Update()
    {
        // BabyManager로 교체
        bool shouldBeActive = BabyManager.Instance.IsBabyInCurrentScene();

        gameObject.SetActive(shouldBeActive);

        if (shouldBeActive)
        {
            transform.position = BabyManager.Instance.babyPosition;
        }

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
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }

    void ShowChoiceUI()
    {
        if (ChoiceUI.Instance != null)
        {
            ChoiceUI.Instance.ShowChoice(
                transform,
                "예",
                "아니",
                OnPickUp,
                OnCancel
            );
        }
    }

    void OnPickUp()
    {
        Debug.Log("아기를 들었습니다!");

        // StatsManager → BabyManager로 교체
        if (BabyManager.Instance != null)
        {
            BabyManager.Instance.CarryBaby();
        }

        gameObject.SetActive(false);
    }

    void OnCancel()
    {
        Debug.Log("취소했습니다.");
    }
}