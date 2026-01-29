using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleManager : MonoBehaviour
{
    [Header("씬 전환 딜레이")]
    public float transitionDelay = 0.6f;

    private bool isTransitioning = false;

    public void StartGame()
    {
        if (!isTransitioning)
        {
            StartCoroutine(StartGameWithDelay());
        }
    }

    IEnumerator StartGameWithDelay()
    {
        isTransitioning = true;
        yield return new WaitForSeconds(transitionDelay);

        // ===== 새 게임 시작 시 모든 게임 데이터 초기화 =====
        PlayerPrefs.DeleteAll(); // 모든 저장 데이터 삭제
        PlayerPrefs.Save();

        // StatsManager도 초기화
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.forestCutscenePlayed = false;
            StatsManager.Instance.babyPickedUp = false;
            StatsManager.Instance.carryingBaby = false;
            StatsManager.Instance.babySceneName = "";
            StatsManager.Instance.babyPosition = Vector3.zero;
        }
        // ========================================

        if (LoadingManager.Instance != null)
        {
            LoadingManager.Instance.LoadScene("WeaponSelectScene", "");
        }
        else
        {
            SceneManager.LoadScene("WeaponSelectScene");
        }
    }

    public void LoadOptions()
    {
        if (!isTransitioning)
        {
            StartCoroutine(LoadOptionsWithDelay());
        }
    }

    IEnumerator LoadOptionsWithDelay()
    {
        isTransitioning = true;
        yield return new WaitForSeconds(transitionDelay);

        Debug.Log("옵션 열기");
    }

    public void QuitGame()
    {
        if (!isTransitioning)
        {
            StartCoroutine(QuitGameWithDelay());
        }
    }

    IEnumerator QuitGameWithDelay()
    {
        isTransitioning = true;
        yield return new WaitForSeconds(transitionDelay);

        Application.Quit();
    }
}