using UnityEngine;
using TMPro;
using System.Collections;

public class CutsceneDialogueUI : MonoBehaviour
{
    public static CutsceneDialogueUI Instance;

    [Header("UI ����")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerText;

    private Coroutine typingCoroutine;
    private Coroutine autoHideCoroutine;

    // SkipToEnd를 위한 현재 다이얼로그 상태 보관
    private string currentFullText;
    private bool currentAutoProgress;
    private float currentAutoDelay;

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
    }

    public void ShowDialogue(string speaker, string text, float typingSpeed, bool autoProgress, float autoDelay)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (autoHideCoroutine != null) StopCoroutine(autoHideCoroutine);

        currentFullText    = text;
        currentAutoProgress = autoProgress;
        currentAutoDelay   = autoDelay;

        speakerText.text = speaker;
        dialoguePanel.SetActive(true);
        typingCoroutine = StartCoroutine(TypeText(text, typingSpeed, autoProgress, autoDelay));
    }

    /// <summary>
    /// 타이핑을 즉시 완료하고 전체 텍스트를 표시합니다.
    /// 클립이 일찍 끝날 때 텍스트가 잘리는 문제를 방지합니다.
    /// </summary>
    public void SkipToEnd()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            dialogueText.text = currentFullText;

            // autoProgress이면 autoHide 시작 (아직 시작 안 됐으면)
            if (currentAutoProgress && autoHideCoroutine == null)
                autoHideCoroutine = StartCoroutine(AutoHide(currentAutoDelay));
        }
        // 타이핑이 이미 끝났거나 패널이 없으면 아무것도 하지 않음
    }

    public void HideDialogue()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (autoHideCoroutine != null) StopCoroutine(autoHideCoroutine);
        dialoguePanel?.SetActive(false);
    }

    IEnumerator TypeText(string text, float speed, bool autoProgress, float autoDelay)
    {
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            float t = 0f;
            while (t < speed)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        if (autoProgress)
            autoHideCoroutine = StartCoroutine(AutoHide(autoDelay));
    }

    IEnumerator AutoHide(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        HideDialogue();
    }
}