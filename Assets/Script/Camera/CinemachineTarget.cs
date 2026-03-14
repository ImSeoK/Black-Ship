using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class CinemachineTarget : MonoBehaviour
{
    private CinemachineCamera vcam;

    void Awake()
    {
        vcam = GetComponent<CinemachineCamera>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        FindPlayer();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayer();
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[CinemachineTarget] Player 없음");
            return;
        }

        vcam.Follow = player.transform;
        Debug.Log("[CinemachineTarget] Player 연결 완료");
    }
}