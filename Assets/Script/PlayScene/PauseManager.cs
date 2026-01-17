using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("Pause Menu")]
    public GameObject pauseMenuUI;
    public KeyCode pauseKey = KeyCode.Escape;

    private bool isPaused = false;

    void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // PauseMenu UI Canvas도 유지
            if (pauseMenuUI != null && pauseMenuUI.transform.root != null)
            {
                DontDestroyOnLoad(pauseMenuUI.transform.root.gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 씬 로드 이벤트 등록
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // 이벤트 해제
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // 씬 로드될 때마다 메뉴 닫기
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        isPaused = false;
        Time.timeScale = 1f;
    }

    void Start()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // DialogueUI 열려있으면 무시
            if (DialogueUI.Instance != null && DialogueUI.Instance.IsDialogueActive())
            {
                return;
            }

            // ChoiceUI 열려있으면 무시
            if (ChoiceUI.Instance != null && ChoiceUI.Instance.IsActive())
            {
                return;
            }

            // 둘 다 없으면 일시정지 토글
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void LoadMainMenu()
    {
        // 일시정지 해제
        Time.timeScale = 1f;
        isPaused = false;

        // 메뉴 닫기
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        // 스폰 정보 삭제
        PlayerPrefs.DeleteKey("LastSpawnPoint");

        // DontDestroyOnLoad 영역 모든 오브젝트 삭제
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene.name == null)
            {
                DestroyImmediate(obj);
            }
        }

        SceneManager.LoadScene("TitleScene");
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료!");
    }
}