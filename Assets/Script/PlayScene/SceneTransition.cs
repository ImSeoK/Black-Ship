using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [Header("전환 설정")]
    public string targetSceneName;
    public string spawnPointName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player 태그 확인
        if (other.CompareTag("Player"))
        {
            // 씬 전환
            if (LoadingManager.Instance != null)
            {
                LoadingManager.Instance.LoadScene(targetSceneName, spawnPointName);
            }
        }
    }
}