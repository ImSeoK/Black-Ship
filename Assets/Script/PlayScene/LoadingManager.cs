using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;

    [Header("Loading UI")]
    public GameObject loadingPanel;
    public Image fadeImage;
    public TMPro.TextMeshProUGUI loadingText;
    public float fadeDuration = 0.5f;

    private bool isLoading = false;
    public static bool IsLoading => Instance != null && Instance.isLoading;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (loadingPanel != null && loadingPanel.transform.parent != null)
            {
                DontDestroyOnLoad(loadingPanel.transform.root.gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
    }

    public void LoadScene(string sceneName, string spawnPointName)
    {
        if (!isLoading)
        {
            StartCoroutine(LoadSceneAsync(sceneName, spawnPointName));
        }
    }

    IEnumerator LoadSceneAsync(string sceneName, string spawnPointName)
    {
        isLoading = true;

        // 게임 일시정지
        Time.timeScale = 0f;

        Debug.Log($"LoadingManager: '{spawnPointName}' 저장 시작"); // 추가
        PlayerPrefs.SetString("LastSpawnPoint", spawnPointName);
        PlayerPrefs.Save(); // 추가 - 강제 저장
        Debug.Log($"저장 완료. 확인: {PlayerPrefs.GetString("LastSpawnPoint", "없음")}"); // 추가

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }

        // 페이드 아웃
        yield return StartCoroutine(Fade(0f, 1f));

        // 씬 비동기 로드
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 카메라 셋업 대기 (unscaledTime 사용)
        yield return StartCoroutine(WaitForRealSeconds(1f));

        // 페이드 인
        yield return StartCoroutine(Fade(1f, 0f));

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }

        // 게임 재개
        Time.timeScale = 1f;

        isLoading = false;
    }

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (fadeImage == null) yield break;

        float elapsedTime = 0f;
        Color fadeColor = fadeImage.color;
        Color textColor = loadingText != null ? loadingText.color : Color.white;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);

            fadeColor.a = alpha;
            fadeImage.color = fadeColor;

            if (loadingText != null)
            {
                textColor.a = alpha;
                loadingText.color = textColor;
            }

            yield return null;
        }

        fadeColor.a = endAlpha;
        fadeImage.color = fadeColor;

        if (loadingText != null)
        {
            textColor.a = endAlpha;
            loadingText.color = textColor;
        }
    }

    IEnumerator WaitForRealSeconds(float seconds)
    {
        float elapsed = 0f;
        while (elapsed < seconds)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
    }
}