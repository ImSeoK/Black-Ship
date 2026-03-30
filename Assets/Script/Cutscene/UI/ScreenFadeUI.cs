using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFadeUI : MonoBehaviour
{
    public static ScreenFadeUI Instance;

    [Header("페이드 이미지 (fullscreen black Image)")]
    public Image fadeImage;

    [Header("설정")]
    public float defaultDuration = 0.5f;

    private Coroutine currentFade;

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
            return;
        }

        // 시작 시 완전 투명
        SetAlpha(0f);

        // EffectBehaviour 이벤트 구독
        EffectBehaviour.OnFadeRequested += HandleFadeRequest;
    }

    void OnDestroy()
    {
        EffectBehaviour.OnFadeRequested -= HandleFadeRequest;
        if (Instance == this) Instance = null;
    }

    void HandleFadeRequest(float from, float to)
    {
        FadeTo(from, to, defaultDuration);
    }

    public void FadeTo(float from, float to, float duration)
    {
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(DoFade(from, to, duration));
    }

    public void FadeOut(float duration = -1f)
    {
        FadeTo(0f, 1f, duration < 0f ? defaultDuration : duration);
    }

    public void FadeIn(float duration = -1f)
    {
        FadeTo(1f, 0f, duration < 0f ? defaultDuration : duration);
    }

    void SetAlpha(float alpha)
    {
        if (fadeImage == null) return;
        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }

    IEnumerator DoFade(float from, float to, float duration)
    {
        SetAlpha(from);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            SetAlpha(Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }

        SetAlpha(to);
        currentFade = null;
    }
}
