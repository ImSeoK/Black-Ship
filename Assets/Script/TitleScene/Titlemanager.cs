using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleManager : MonoBehaviour
{
    [Header("¾À ÀüÈ¯ µô·¹ÀÌ")]
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
            LoadingManager.Instance.LoadScene("BaseCamp", "DefaultSpawn"); // spawnPointName ºó ¹®ÀÚ¿­
        }
        else
        {
            SceneManager.LoadScene("BaseCamp");
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

        Debug.Log("¿É¼Ç ¿­±â");
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