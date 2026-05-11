using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    [Header("기본 스폰 설정")]
    public string defaultSpawnPointName = "DefaultSpawn"; // BaseCamp 기본 스폰

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // BaseCamp 씬일 때만 기본 스폰 체크
        if (scene.name == "BaseCamp")
        {
            CheckSpawnPointForBaseCamp();
        }
        else
        {
            CheckSpawnPoint();
        }
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "BaseCamp")
        {
            CheckSpawnPointForBaseCamp();
        }
        else
        {
            CheckSpawnPoint();
        }
    }

    void CheckSpawnPointForBaseCamp()
    {
        string spawnPointName = PlayerPrefs.GetString("LastSpawnPoint", "");
        Debug.Log($"BaseCamp 스폰 체크: '{spawnPointName}'");

        // 비어있으면 기본 위치 사용
        if (string.IsNullOrEmpty(spawnPointName))
        {
            spawnPointName = defaultSpawnPointName;
            Debug.Log($"기본 스폰 사용: {spawnPointName}");
        }

        GameObject spawnPoint = GameObject.Find(spawnPointName);

        if (spawnPoint != null)
        {
            Vector3 spawnPos = spawnPoint.transform.position;
            spawnPos.y += 0.5f;
            transform.position = spawnPos;
            Debug.Log($" 스폰 성공: {spawnPointName}");
        }
        else
        {
            Debug.LogError($" '{spawnPointName}' 못 찾음!");
        }

        PlayerPrefs.DeleteKey("LastSpawnPoint");
    }

    void CheckSpawnPoint()
    {
        string spawnPointName = PlayerPrefs.GetString("LastSpawnPoint", "");

        if (!string.IsNullOrEmpty(spawnPointName))
        {
            GameObject spawnPoint = GameObject.Find(spawnPointName);

            if (spawnPoint != null)
            {
                Vector3 spawnPos = spawnPoint.transform.position;
                // spawnPos.y += 0.5f; ← 이 줄 삭제 또는 0으로
                transform.position = spawnPos;
                Debug.Log($"스폰 성공: {spawnPointName}");
            }
            else
            {
                Debug.LogError($"'{spawnPointName}' 못 찾음!");
            }

            PlayerPrefs.DeleteKey("LastSpawnPoint");
        }
    }
}