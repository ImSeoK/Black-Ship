using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// 3구간(이송 구간) 전용 무전 수신 UI와 튜토리얼 힌트를 담당합니다.
/// PrologueTriggerZone 또는 PrologueManager에서 호출합니다.
/// </summary>
public class RadioTutorialUI : MonoBehaviour
{
    public static RadioTutorialUI Instance;

    [Header("무전 패널")]
    [Tooltip("무전 수신 전체 패널 오브젝트")]
    public GameObject radioPanel;
    [Tooltip("발신자 이름 텍스트 (예: 지원팀, 본부)")]
    public TextMeshProUGUI callerText;
    [Tooltip("수신 메시지 텍스트 (타이핑 효과)")]
    public TextMeshProUGUI radioMessageText;
    [Tooltip("무전기 신호 아이콘 (선택)")]
    public Image signalIcon;
    [Tooltip("무전기 수신음 / 잡음 SFX")]
    public AudioClip staticNoiseSFX;

    [Header("튜토리얼 힌트 패널")]
    [Tooltip("조작 힌트 전체 패널 오브젝트")]
    public GameObject hintPanel;
    [Tooltip("조작 힌트 텍스트")]
    public TextMeshProUGUI hintText;

    [Header("설정")]
    [Tooltip("무전 메시지 타이핑 속도 (초/글자)")]
    public float radioTypingSpeed = 0.04f;
    [Tooltip("메시지 완료 후 자동 숨김 대기 시간(초)")]
    public float radioAutoHideDelay = 3f;

    private Coroutine typingCoroutine;
    private Coroutine autoHideCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        radioPanel?.SetActive(false);
        hintPanel?.SetActive(false);
    }

    // ================================================================
    // 무전 메시지
    // ================================================================

    /// <summary>
    /// 무전 수신 패널을 열고 타이핑 효과로 메시지를 표시합니다.
    /// </summary>
    /// <param name="caller">발신자 이름</param>
    /// <param name="message">수신 메시지</param>
    /// <param name="autoHide">true면 메시지 완료 후 자동 숨김</param>
    public void ShowRadioMessage(string caller, string message, bool autoHide = true)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (autoHideCoroutine != null) StopCoroutine(autoHideCoroutine);

        if (callerText != null) callerText.text = caller;
        radioPanel?.SetActive(true);

        if (staticNoiseSFX != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(staticNoiseSFX);

        typingCoroutine = StartCoroutine(TypeRadioMessage(message, autoHide));
    }

    /// <summary>무전 패널을 즉시 숨깁니다.</summary>
    public void HideRadio()
    {
        if (typingCoroutine != null) { StopCoroutine(typingCoroutine); typingCoroutine = null; }
        if (autoHideCoroutine != null) { StopCoroutine(autoHideCoroutine); autoHideCoroutine = null; }
        radioPanel?.SetActive(false);
    }

    IEnumerator TypeRadioMessage(string message, bool autoHide)
    {
        if (radioMessageText != null) radioMessageText.text = "";

        foreach (char c in message)
        {
            if (radioMessageText != null) radioMessageText.text += c;

            float t = 0f;
            while (t < radioTypingSpeed)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        typingCoroutine = null;

        if (autoHide)
            autoHideCoroutine = StartCoroutine(AutoHideRadio());
    }

    IEnumerator AutoHideRadio()
    {
        yield return new WaitForSecondsRealtime(radioAutoHideDelay);
        HideRadio();
    }

    // ================================================================
    // 튜토리얼 힌트
    // ================================================================

    /// <summary>조작 힌트 텍스트를 변경하고 패널을 표시합니다.</summary>
    public void ShowTutorialHint(string hint)
    {
        if (hintText != null) hintText.text = hint;
        hintPanel?.SetActive(true);
    }

    /// <summary>조작 힌트 패널을 숨깁니다.</summary>
    public void HideTutorialHint()
    {
        hintPanel?.SetActive(false);
    }

    /// <summary>3구간 진입 시 초기 이동 힌트를 표시합니다.</summary>
    public void ShowInitialHints()
    {
        ShowTutorialHint("[ WASD ] 이동    [ Shift ] 달리기");
    }

    /// <summary>무전 패널과 힌트 패널을 모두 숨깁니다.</summary>
    public void HideAll()
    {
        HideRadio();
        HideTutorialHint();
    }
}
