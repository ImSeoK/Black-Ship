using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    public string targetSceneName; // 이동할 씬 이름
    public string spawnPointName = "SpawnPoint"; // 도착 위치 이름

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.F;
    public float interactionRange = 1.5f;
    public LayerMask playerLayer;

    private bool playerInRange = false;
    private Transform doorTransform;

    void Start()
    {
        doorTransform = transform;
    }

    void Update()
    {
        // 로딩 중이면 입력 무시
        if (LoadingManager.IsLoading) return;

        // 플레이어가 범위 안에 있고 F키 누르면
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            LoadScene();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (InteractionUI.Instance != null)
            {
                InteractionUI.Instance.ShowPrompt(doorTransform, true); // true = F키 UI
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (InteractionUI.Instance != null)
            {
                InteractionUI.Instance.HidePrompt();
            }
        }
    }

    void LoadScene()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("Target Scene Name이 설정되지 않았습니다!");
            return;
        }

        // LoadingManager 사용
        if (LoadingManager.Instance != null)
        {
            LoadingManager.Instance.LoadScene(targetSceneName, spawnPointName);
        }
        else
        {
            Debug.LogError("LoadingManager가 없습니다!");
        }
    }
}