using UnityEngine;
using UnityEngine.Playables;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;

    [Header("컷씬 설정")]
    public PlayableDirector director;
    public string cutsceneID;
    public bool playOnce = true;
    public bool playOnStart = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (playOnStart)
            StartCoroutine(PlayNextFrame());
    }

    IEnumerator PlayNextFrame()
    {
        yield return null;
        Play(director, cutsceneID, playOnce);
    }

    public void Play(PlayableDirector targetDirector, string id, bool once)
    {
        if (targetDirector == null)
        {
            Debug.LogWarning($"[CutsceneManager] {id} - Director 없음");
            return;
        }

        if (once && HasPlayed(id))
        {
            Debug.Log($"[CutsceneManager] {id} 스킵");
            return;
        }

        StartCutscene(targetDirector, id, once);
    }

    void StartCutscene(PlayableDirector targetDirector, string id, bool once)
    {
        DisablePlayer();
        targetDirector.Play();
        targetDirector.stopped += (d) => OnFinished(d, id, once);
    }

    void OnFinished(PlayableDirector d, string id, bool once)
    {
        EnablePlayer();
        ResetCutsceneCameras(d);
        if (once) MarkPlayed(id);
        d.stopped -= (dir) => OnFinished(dir, id, once);
    }

    bool HasPlayed(string id)
    {
        return PlayerPrefs.GetInt($"Cutscene_{id}", 0) == 1;
    }

    void MarkPlayed(string id)
    {
        PlayerPrefs.SetInt($"Cutscene_{id}", 1);
        PlayerPrefs.Save();
    }

    void DisablePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        foreach (var script in player.GetComponents<MonoBehaviour>())
            script.enabled = false;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    void EnablePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        foreach (var script in player.GetComponents<MonoBehaviour>())
            script.enabled = true;
    }

    void ResetCutsceneCameras(PlayableDirector d)
    {
        foreach (var cam in d.GetComponentsInChildren<Component>())
        {
            if (cam.GetType().Name != "CinemachineCamera") continue;
            var prop = cam.GetType().GetProperty("Priority");
            if (prop == null) continue;
            var priority = prop.GetValue(cam);
            priority?.GetType().GetProperty("Value")?.SetValue(priority, 0);
        }
    }
}