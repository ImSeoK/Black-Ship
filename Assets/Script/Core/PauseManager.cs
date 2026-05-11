using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("Pause Menu")]
    public GameObject pauseMenuUI;

    private bool isPaused = false;

    void Awake()
    {
        // �̱���
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // PauseMenu UI Canvas�� ����
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

        // �� �ε� �̺�Ʈ ���
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // �̺�Ʈ ����
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // �� �ε�� ������ �޴� �ݱ�
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
        if (InputManager.Instance != null && InputManager.Instance.IsPausePressed)
        {
            // DialogueUI ���������� ����
            if (DialogueUI.Instance != null && DialogueUI.Instance.IsDialogueActive())
            {
                return;
            }

            // ChoiceUI ���������� ����
            if (ChoiceUI.Instance != null && ChoiceUI.Instance.IsActive())
            {
                return;
            }

            // �� �� ������ �Ͻ����� ���
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
        // �Ͻ����� ����
        Time.timeScale = 1f;
        isPaused = false;

        // �޴� �ݱ�
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        // ���� ���� ����
        PlayerPrefs.DeleteKey("LastSpawnPoint");

        // DontDestroyOnLoad ���� ��� ������Ʈ ����
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
        Debug.Log("���� ����!");
    }
}