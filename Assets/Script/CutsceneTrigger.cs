using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("컷씬 설정")]
    public PlayableDirector timeline;
    public Transform teleportPosition;
    public bool playOnce = true;

    private bool hasPlayed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (playOnce && hasPlayed) return;

            StartCoroutine(TeleportWithLoading(other.gameObject));

            hasPlayed = true;
        }
    }

    IEnumerator TeleportWithLoading(GameObject player)
    {
        BindPlayerToTimeline(player);

        // 페이드 아웃 (씬 전환처럼)
        if (LoadingManager.Instance != null)
        {
            LoadingManager.Instance.loadingPanel.SetActive(true);
            yield return StartCoroutine(LoadingManager.Instance.Fade(0f, 1f));
        }

        // 텔레포트
        if (teleportPosition != null)
        {
            player.transform.position = teleportPosition.position;
        }

        yield return new WaitForSecondsRealtime(0.5f);

        // 페이드 인 (씬 전환처럼)
        if (LoadingManager.Instance != null)
        {
            yield return StartCoroutine(LoadingManager.Instance.Fade(1f, 0f));
            LoadingManager.Instance.loadingPanel.SetActive(false);
        }

        yield return new WaitForSecondsRealtime(0.3f);

        // 컷씬 시작
        if (timeline != null)
        {
            timeline.Play();
        }
    }

    void BindPlayerToTimeline(GameObject player)
    {
        if (timeline == null) return;

        // Timeline의 모든 Track 검색
        TimelineAsset timelineAsset = timeline.playableAsset as TimelineAsset;

        foreach (var track in timelineAsset.GetOutputTracks())
        {
            // Animation Track 찾기
            if (track is AnimationTrack)
            {
                // Player 바인딩
                timeline.SetGenericBinding(track, player);
            }
        }
    }
}