using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    [Header("재생 설정")]
    public PlayableDirector timeline;
    public string cutsceneID = "OpeningCutscene";

    [Header("옵션")]
    public bool playOnStart = true;
    public bool playOnce = true;

    private bool hasPlayed = false;

    void Start()
    {
        if (playOnStart)
        {
            PlayCutscene();
        }
    }

    public void PlayCutscene()
    {
        if (hasPlayed) return;
        if (timeline == null)
        {
            Debug.LogWarning($"[{cutsceneID}] Timeline이 할당되지 않았습니다.");
            return;
        }

        if (playOnce && HasPlayedBefore())
        {
            Debug.Log($"[{cutsceneID}] 이미 재생됨 - 스킵");
            gameObject.SetActive(false);
            return;
        }

        hasPlayed = true;
        StartCutscene();
    }

    void StartCutscene()
    {
        DisablePlayerControl();
        Time.timeScale = 0f;

        timeline.Play();
        timeline.stopped += OnCutsceneFinished;
    }

    void OnCutsceneFinished(PlayableDirector director)
    {
        EnablePlayerControl();
        Time.timeScale = 1f;

        // 컷씬 카메라 Priority 초기화 (수정!)
        ResetCutsceneCameras();

        if (playOnce)
        {
            MarkAsPlayed();
        }

        director.stopped -= OnCutsceneFinished;
    }

    void ResetCutsceneCameras()
    {
        // Reflection 사용해서 타입 찾기
        var cams = GetComponentsInChildren<Component>();
        foreach (var cam in cams)
        {
            if (cam.GetType().Name == "CinemachineCamera")
            {
                var priorityField = cam.GetType().GetProperty("Priority");
                if (priorityField != null)
                {
                    var priority = priorityField.GetValue(cam);
                    var valueField = priority.GetType().GetProperty("Value");
                    if (valueField != null)
                    {
                        valueField.SetValue(priority, 0);
                    }
                }
            }
        }
    }

    void DisablePlayerControl()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            script.enabled = false;
        }

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
            if (cameraFollow != null) cameraFollow.enabled = false;
        }
    }

    void EnablePlayerControl()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            script.enabled = true;
        }

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
            if (cameraFollow != null) cameraFollow.enabled = true;
        }
    }

    bool HasPlayedBefore()
    {
        if (StatsManager.Instance == null) return false;

        switch (cutsceneID)
        {
            case "OpeningCutscene":
                return StatsManager.Instance.openingCutscenePlayed;
            case "ForestCutscene":
                return StatsManager.Instance.forestCutscenePlayed;
            default:
                return false;
        }
    }

    void MarkAsPlayed()
    {
        if (StatsManager.Instance == null) return;

        switch (cutsceneID)
        {
            case "OpeningCutscene":
                StatsManager.Instance.openingCutscenePlayed = true;
                break;
            case "ForestCutscene":
                StatsManager.Instance.forestCutscenePlayed = true;
                break;
        }

        StatsManager.Instance.SaveState();
    }

    void OnDestroy()
    {
        if (timeline != null)
        {
            timeline.stopped -= OnCutsceneFinished;
        }
    }
}