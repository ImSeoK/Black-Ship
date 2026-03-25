using UnityEngine;
using System.Collections;

public class LetterboxUI : MonoBehaviour
{
    public static LetterboxUI Instance;

    [Header("바 RectTransform")]
    public RectTransform topBar;
    public RectTransform bottomBar;

    [Header("설정")]
    public float barHeight = 100f;
    public float animDuration = 0.4f;

    private Coroutine currentAnim;

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

        // 시작 시 바를 화면 밖으로 숨김
        SetBarsImmediate(hidden: true);
    }

    public void Show()
    {
        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(Animate(show: true));
    }

    public void Hide()
    {
        if (this == null || !gameObject) return;
        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(Animate(show: false));
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    IEnumerator Animate(bool show)
    {
        // show=true  → 바가 화면 안으로 슬라이드 인
        // show=false → 바가 화면 밖으로 슬라이드 아웃
        float topHidden    = barHeight;   // 위 바의 숨겨진 anchoredPositionY
        float topVisible   = 0f;
        float bottomHidden = -barHeight;  // 아래 바의 숨겨진 anchoredPositionY
        float bottomVisible = 0f;

        float topStart    = show ? topHidden    : topVisible;
        float topEnd      = show ? topVisible   : topHidden;
        float bottomStart = show ? bottomHidden : bottomVisible;
        float bottomEnd   = show ? bottomVisible : bottomHidden;

        float elapsed = 0f;
        while (elapsed < animDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / animDuration);

            if (topBar != null)
            {
                Vector2 pos = topBar.anchoredPosition;
                pos.y = Mathf.Lerp(topStart, topEnd, t);
                topBar.anchoredPosition = pos;
            }
            if (bottomBar != null)
            {
                Vector2 pos = bottomBar.anchoredPosition;
                pos.y = Mathf.Lerp(bottomStart, bottomEnd, t);
                bottomBar.anchoredPosition = pos;
            }

            yield return null;
        }

        // 최종 위치 정확히 고정
        SetBarsImmediate(hidden: !show);
        currentAnim = null;
    }

    void SetBarsImmediate(bool hidden)
    {
        if (topBar != null)
        {
            Vector2 pos = topBar.anchoredPosition;
            pos.y = hidden ? barHeight : 0f;
            topBar.anchoredPosition = pos;
        }
        if (bottomBar != null)
        {
            Vector2 pos = bottomBar.anchoredPosition;
            pos.y = hidden ? -barHeight : 0f;
            bottomBar.anchoredPosition = pos;
        }
    }
}
