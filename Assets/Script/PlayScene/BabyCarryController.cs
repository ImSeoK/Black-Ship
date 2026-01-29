using UnityEngine;

public class BabyCarryController : MonoBehaviour
{
    [Header("아이 스프라이트")]
    public GameObject babySprite;

    [Header("키 설정")]
    public KeyCode putDownKey = KeyCode.R; // Q → R로 변경

    void Update()
    {
        // 아이 스프라이트 표시/숨김
        if (babySprite != null)
        {
            babySprite.SetActive(StatsManager.Instance != null &&
                                 StatsManager.Instance.carryingBaby);
        }

        // R키로 아이 내려놓기
        if (Input.GetKeyDown(putDownKey))
        {
            if (StatsManager.Instance != null &&
                StatsManager.Instance.carryingBaby)
            {
                PutDownBaby();
            }
        }
    }

    void PutDownBaby()
    {
        Vector3 dropPosition = transform.position;
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        StatsManager.Instance.PutDownBaby(dropPosition, currentScene);

        // 즉시 BabyOnGround 활성화
        ActivateBabyOnGround();

        Debug.Log($"아이를 {currentScene}에 두었습니다!");
    }

    void ActivateBabyOnGround()
    {
        // FindFirstObjectByType로 변경
        BabyOnGround babyOnGround = FindFirstObjectByType<BabyOnGround>(FindObjectsInactive.Include);

        if (babyOnGround != null)
        {
            babyOnGround.gameObject.SetActive(true);
            babyOnGround.transform.position = StatsManager.Instance.babyPosition;
        }
    }
}