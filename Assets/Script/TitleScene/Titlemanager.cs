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

        PlayerPrefs.DeleteKey("LastSpawnPoint");

        if (LoadingManager.Instance != null)
        {
            // WeaponSelectScene으로 변경!
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