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

        // 시작 시 완전 검정 (타임라인 FadeIn 클립이 직접 알파를 구동함)
        SetAlpha(1f);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
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

    public void SetAlpha(float alpha)
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
