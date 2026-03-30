using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// 5구간(능력 발현) 컷씬 이후 UTO 타이틀 로고 연출을 담당합니다.
/// PrologueManager에서 PlayTitleReveal()을 호출하면 페이드인 → 유지 → 페이드아웃 순으로 재생됩니다.
/// </summary>
public class UTOTitleUI : MonoBehaviour
{
    public static UTOTitleUI Instance;

    [Header("타이틀 UI 요소")]
    [Tooltip("전체 타이틀 그룹 (알파 페이드 제어용 CanvasGroup)")]
    public CanvasGroup titleGroup;
    [Tooltip("UTO 로고 이미지")]
    public Image logoImage;
    [Tooltip("서브타이틀 텍스트 (선택, 비워두면 표시 안 함)")]
    public TextMeshProUGUI subtitleText;

    [Header("연출 타이밍")]
    [Tooltip("로고 페이드인 시간(초)")]
    public float fadeInDuration = 1.5f;
    [Tooltip("로고가 완전히 보이는 유지 시간(초)")]
    public float holdDuration = 2.5f;
    [Tooltip("로고 페이드아웃 시간(초)")]
    public float fadeOutDuration = 1.0f;

    [Header("오디오")]
    [Tooltip("타이틀 등장 SFX (선택)")]
    public AudioClip revealSFX;

    private Coroutine revealCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 시작 시 완전히 숨김
        if (titleGroup != null)
        {
            titleGroup.alpha = 0f;
            titleGroup.gameObject.SetActive(false);
        }
    }

    // ================================================================
    // 공개 API
    // ================================================================

    /// <summary>
    /// 타이틀 로고 연출을 재생합니다.
    /// 완료 시 onComplete 콜백이 호출됩니다.
    /// </summary>
    public void PlayTitleReveal(System.Action onComplete = null)
    {
        if (revealCoroutine != null) StopCoroutine(revealCoroutine);
        revealCoroutine = StartCoroutine(RevealSequence(onComplete));
    }

    /// <summary>연출을 즉시 중단하고 숨깁니다.</summary>
    public void HideImmediate()
    {
        if (revealCoroutine != null)
        {
            StopCoroutine(revealCoroutine);
            revealCoroutine = null;
        }

        if (titleGroup != null)
        {
            titleGroup.alpha = 0f;
            titleGroup.gameObject.SetActive(false);
        }
    }

    // ================================================================
    // 내부 로직
    // ================================================================

    IEnumerator RevealSequence(System.Action onComplete)
    {
        if (titleGroup == null)
        {
            Debug.LogWarning("[UTOTitleUI] CanvasGroup 미할당 → 타이틀 연출 스킵");
            onComplete?.Invoke();
            yield break;
        }

        titleGroup.alpha = 0f;
        titleGroup.gameObject.SetActive(true);

        if (revealSFX != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(revealSFX);

        // 페이드 인
        yield return FadeGroup(0f, 1f, fadeInDuration);

        // 유지
        yield return new WaitForSecondsRealtime(holdDuration);

        // 페이드 아웃
        yield return FadeGroup(1f, 0f, fadeOutDuration);

        titleGroup.gameObject.SetActive(false);
        revealCoroutine = null;

        onComplete?.Invoke();
    }

    IEnumerator FadeGroup(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            titleGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        titleGroup.alpha = to;
    }
}
