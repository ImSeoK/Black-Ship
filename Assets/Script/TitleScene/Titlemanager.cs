using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("Scene Settings")]
    public string gameSceneName = "BaseCamp";

    public void StartGame()
    {
        Debug.Log("게임 시작!");

        // 이전 스폰 정보 삭제
        PlayerPrefs.DeleteKey("LastSpawnPoint");

        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}