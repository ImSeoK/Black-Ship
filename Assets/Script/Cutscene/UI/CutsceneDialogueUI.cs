using UnityEngine;
using UnityEngine.Playables;
using TMPro;
using System.Collections;

public class CutsceneDialogueUI : MonoBehaviour
{
    public static CutsceneDialogueUI Instance;

    [Header("UI 참조")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    [Tooltip("타이핑 완료 후 표시할 진행 표시(다음 버튼 화살표 등)")]
    public GameObject nextIndicator;

    private Coroutine typingCoroutine;

    private string currentFullText;
    private bool isTyping = false;

    public bool IsDirectorPausedByUs { get; private set; } = false;

    private PlayableDirector currentDirector;
    private double currentClipEndTime;

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
        dialoguePanel?.SetActive(false);
        nextIndicator?.SetActive(false);
    }

    // ================================================================
    // 공개 API
    // ================================================================

    /// <summary>
    /// 컷씬(Timeline) 구간에서 호출. director를 전달하면 Pause/Resume을 자동 처리합니다.
    /// director가 null이면 일반 대화 모드로 동작합니다.
    /// </summary>
    public void ShowDialogue(string speaker, string text, float typingSpeed,
                             SpeakerType speakerType, PlayableDirector director, double clipEndTime)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        currentFullText    = text;
        currentDirector    = director;
        currentClipEndTime = clipEndTime;
        isTyping           = false;

        if (speakerText != null) speakerText.text = speaker;
        if (dialogueText != null) dialogueText.color = SpeakerColors.GetColor(speakerType);

        dialoguePanel?.SetActive(true);
        nextIndicator?.SetActive(false);

        // director가 있으면 즉시 속도 0으로 동결 (PlayState 유지, OnBehaviourPause 미발화)
        if (currentDirector != null)
        {
            IsDirectorPausedByUs = true;
            if (currentDirector.playableGraph.IsValid())
                currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
        }

        typingCoroutine = StartCoroutine(TypeText(text, typingSpeed));
    }

    /// <summary>
    /// 타이핑을 즉시 완료합니다. OnBehaviourPause에서 호출 가능.
    /// </summary>
    public void SkipToEnd()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        if (dialogueText != null) dialogueText.text = currentFullText;
        isTyping = false;
        nextIndicator?.SetActive(true);
    }

    /// <summary>
    /// 대화 패널을 숨기고 director를 재개합니다.
    /// </summary>
    public void HideDialogue()
    {
        if (typingCoroutine != null) { StopCoroutine(typingCoroutine); typingCoroutine = null; }
        dialoguePanel?.SetActive(false);
        nextIndicator?.SetActive(false);
        isTyping = false;

        if (currentDirector != null && IsDirectorPausedByUs)
        {
            IsDirectorPausedByUs = false;
            var d = currentDirector;
            currentDirector = null; // 먼저 null 처리: d.time 점프 시 콜백이 동기 발화되므로

            if (d.playableGraph.IsValid())
            {
                d.playableGraph.GetRootPlayable(0).SetSpeed(1);
                d.time = currentClipEndTime; // 클립 끝으로 점프 → 다음 클립 즉시 시작
            }
        }
    }

    // ================================================================
    // 입력 처리
    // ================================================================

    void Update()
    {
        if (dialoguePanel == null || !dialoguePanel.activeSelf) return;
        if (!Input.GetKeyDown(KeyCode.Space)) return;

        if (isTyping)
        {
            // 타이핑 중 → 즉시 완성 (아직 director 재개하지 않음)
            SkipToEnd();
            return;
        }

        // 타이핑 완료 상태 → 대화 닫고 director 재개
        HideDialogue();
    }

    // ================================================================
    // 내부 로직
    // ================================================================

    IEnumerator TypeText(string text, float speed)
    {
        isTyping = true;
        if (dialogueText != null) dialogueText.text = "";

        foreach (char c in text)
        {
            if (dialogueText != null) dialogueText.text += c;

            float t = 0f;
            while (t < speed)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        isTyping = false;
        typingCoroutine = null;
        nextIndicator?.SetActive(true);
    }
}
