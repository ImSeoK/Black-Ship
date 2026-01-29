using UnityEngine;
using UnityEngine.Playables;
using System.Collections;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("컷씬 설정")]
    public PlayableDirector timeline;
    public Transform teleportPosition;
    public bool playOnce = true;

    [Header("컷씬 ID")]
    public string cutsceneID = "ForestCutscene";

    private bool hasTriggered = false; // 추가!

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 이미 트리거됨
            if (hasTriggered) return;

            // 이미 본 컷씬인지 체크
            bool alreadyPlayed = playOnce && HasPlayedCutscene();

            // 트리거 발동 (한 번만)
            hasTriggered = true;

            StartCoroutine(TeleportWithLoading(other.gameObject, alreadyPlayed));
        }
    }

    bool HasPlayedCutscene()
    {
        if (StatsManager.Instance == null) return false;

        switch (cutsceneID)
        {
            case "ForestCutscene":
                return StatsManager.Instance.forestCutscenePlayed;
            default:
                return false;
        }
    }

    void MarkCutsceneAsPlayed()
    {
        if (StatsManager.Instance == null) return;

        switch (cutsceneID)
        {
            case "ForestCutscene":
                StatsManager.Instance.forestCutscenePlayed = true;
                break;
        }

        StatsManager.Instance.SaveState();
    }

    IEnumerator TeleportWithLoading(GameObject player, bool skipCutscene)
    {
        BindPlayerToTimeline(player);

        // 페이드 아웃
        if (LoadingManager.Instance != null)
        {
            LoadingManager.Instance.loadingPanel.SetActive(true);
            yield return StartCoroutine(LoadingManager.Instance.Fade(0f, 1f));
        }

        // 텔레포트 (항상 실행)
        if (teleportPosition != null)
        {
            player.transform.position = teleportPosition.position;
        }

        yield return new WaitForSecondsRealtime(0.5f);

        // 페이드 인
        if (LoadingManager.Instance != null)
        {
            yield return StartCoroutine(LoadingManager.Instance.Fade(1f, 0f));
            LoadingManager.Instance.loadingPanel.SetActive(false);
        }

        yield return new WaitForSecondsRealtime(0.3f);

        // 컷씬 재생 여부 결정
        if (!skipCutscene && timeline != null)
        {
            timeline.Play();
            MarkCutsceneAsPlayed();
        }
        else
        {
            Debug.Log($"{cutsceneID} 스킵됨 - 텔레포트만 실행");
        }
    }

    void BindPlayerToTimeline(GameObject player)
    {
        if (timeline == null) return;

        UnityEngine.Timeline.TimelineAsset timelineAsset = timeline.playableAsset as UnityEngine.Timeline.TimelineAsset;

        foreach (var track in timelineAsset.GetOutputTracks())
        {
            if (track is UnityEngine.Timeline.AnimationTrack)
            {
                timeline.SetGenericBinding(track, player);
            }
        }
    }
}