using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleManager : MonoBehaviour
{
    public float transitionDelay = 0.6f;

    private bool isTransitioning = false;

    public void StartGame()
    {
        if (!isTransitioning)
            StartCoroutine(StartGameWithDelay());
    }

    IEnumerator StartGameWithDelay()
    {
        isTransitioning = true;
        yield return new WaitForSeconds(transitionDelay);

        // 전체 초기화 (컷씬 기록 포함 전부 삭제됨)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // BabyManager 초기화
        if (BabyManager.Instance != null)
        {
            BabyManager.Instance.babyPickedUp = false;
            BabyManager.Instance.carryingBaby = false;
            BabyManager.Instance.babySceneName = "";
            BabyManager.Instance.babyPosition = Vector3.zero;
        }

        if (LoadingManager.Instance != null)
            LoadingManager.Instance.LoadScene("WeaponSelectScene", "");
        else
            SceneManager.LoadScene("WeaponSelectScene");
    }

    public void LoadOptions()
    {
        if (!isTransitioning)
            StartCoroutine(LoadOptionsWithDelay());
    }

    IEnumerator LoadOptionsWithDelay()
    {
        isTransitioning = true;
        yield return new WaitForSeconds(transitionDelay);
    }

    public void QuitGame()
    {
        if (!isTransitioning)
            StartCoroutine(QuitGameWithDelay());
    }

    IEnumerator QuitGameWithDelay()
    {
        isTransitioning = true;
        yield return new WaitForSeconds(transitionDelay);
        Application.Quit();
    }
}