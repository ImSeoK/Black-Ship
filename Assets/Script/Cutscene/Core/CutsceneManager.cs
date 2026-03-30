using UnityEngine;
using UnityEngine.Playables;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;

    [Header("자동 재생 (선택)")]
    public PlayableDirector director;
    public CutsceneData startData;
    public bool playOnStart = false;

    [Header("개발용 (빌드 전 반드시 해제)")]
    public bool debugIgnorePlayOnce = false;

    // 문제2 수정: 컷씬 시작 전 활성 상태였던 컴포넌트만 기록
    private readonly List<MonoBehaviour> originallyEnabled = new List<MonoBehaviour>();

    // 문제1 수정: 델리게이트를 필드로 저장해 동일 인스턴스로 구독/해제
    private System.Action<PlayableDirector> onStoppedCallback;

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
        Play(director, startData, null);
    }

    // onComplete: 컷씬 종료 시 호출할 콜백 (선택적)
    public void Play(PlayableDirector targetDirector, CutsceneData data, System.Action onComplete = null)
    {
        if (targetDirector == null || data == null)
        {
            Debug.LogWarning("[CutsceneManager] Director 또는 CutsceneData 없음");
            return;
        }

        if (data.playOnce && !debugIgnorePlayOnce && HasPlayed(data.cutsceneID))
        {
            Debug.Log($"[CutsceneManager] {data.cutsceneID} 스킵");
            return;
        }

        if (data.timelineAsset != null)
            targetDirector.playableAsset = data.timelineAsset;

        StartCutscene(targetDirector, data.cutsceneID, data.playOnce, data.keepLetterboxOnEnd, onComplete);
    }

    void StartCutscene(PlayableDirector targetDirector, string id, bool once, bool keepLetterbox, System.Action onComplete)
    {
        DisablePlayer();
        if (LetterboxUI.Instance != null) LetterboxUI.Instance.Show();
        onStoppedCallback = (d) => OnFinished(d, id, once, keepLetterbox, onComplete);
        targetDirector.stopped += onStoppedCallback;
        targetDirector.Play();
    }

    void OnFinished(PlayableDirector d, string id, bool once, bool keepLetterbox, System.Action onComplete)
    {
        d.stopped -= onStoppedCallback;
        onStoppedCallback = null;

        EnablePlayer();
        if (!keepLetterbox && LetterboxUI.Instance != null) LetterboxUI.Instance.Hide();
        ResetCutsceneCameras(d);
        if (once) MarkPlayed(id);
        onComplete?.Invoke();
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

    [ContextMenu("컷씬 기록 전체 초기화")]
    void ResetAllCutsceneRecords()
    {
        // Cutscene_ 접두사 키만 삭제하기 위해 알려진 ID를 수동 삭제
        // (PlayerPrefs는 key 목록 조회 API가 없으므로 전체 삭제 사용)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("[CutsceneManager] 모든 컷씬 기록 초기화 완료");
    }

    void DisablePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // 문제2 수정: 원래 활성 상태인 것만 기록하고 비활성화
        originallyEnabled.Clear();
        foreach (var script in player.GetComponents<MonoBehaviour>())
        {
            if (script.enabled)
            {
                originallyEnabled.Add(script);
                script.enabled = false;
            }
        }

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    void EnablePlayer()
    {
        // 문제2 수정: 원래 켜져 있던 것만 복원
        foreach (var script in originallyEnabled)
        {
            if (script != null)
                script.enabled = true;
        }
        originallyEnabled.Clear();
    }

    void ResetCutsceneCameras(PlayableDirector d)
    {
        foreach (var vcam in d.GetComponentsInChildren<CinemachineCamera>())
        {
            var p = vcam.Priority;
            p.Value = 0;
            vcam.Priority = p;
        }
    }
}
