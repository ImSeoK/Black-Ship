using UnityEngine;
using UnityEngine.Playables;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("컷씬 설정")]
    public PlayableDirector director;  // 씬의 PlayableDirector (재생 주체)
    public CutsceneData data;          // 에셋의 CutsceneData

    // hasTriggered 역할: 재생 중 중복 발동 방지 (트리거 영역 내 중복 호출 차단)
    // playOnce 역할: 게임 전체에서 한 번만 재생 (PlayerPrefs 영구 기록)
    // → playOnce = false이면 컷씬 종료 후 hasTriggered 리셋 → 재진입 시 다시 재생 가능
    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (hasTriggered) return;

        hasTriggered = true;
        CutsceneManager.Instance?.Play(director, data, OnCutsceneFinished);
    }

    void OnCutsceneFinished()
    {
        // playOnce = false(반복 재생)이면 재진입 가능하도록 리셋
        if (data != null && !data.playOnce)
            hasTriggered = false;
    }
}
