using UnityEngine;

public class SceneBGMController : MonoBehaviour
{
    [Header("BGM for this Scene")]
    public AudioClip sceneBGM;
    public bool playOnStart = true;
    public bool stopWhenSceneChanges = false; // ¾À ³ª°¥ ¶§ BGM ¸ØÃß±â

    void Start()
    {
        if (AudioManager.Instance == null) return;

        if (playOnStart && sceneBGM != null)
        {
            AudioManager.Instance.PlayBGM(sceneBGM);
        }
    }

    void OnDestroy()
    {
        // ¾À ÀüÈ¯ ½Ã BGM ¸ØÃß±â
        if (stopWhenSceneChanges && AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
        }
    }
}