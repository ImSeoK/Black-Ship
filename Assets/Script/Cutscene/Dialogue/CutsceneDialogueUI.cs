using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// 컷씬 전용 대사 UI
/// 자동 진행, 리치 텍스트, 속도 조절 지원
/// </summary>
public class CutsceneDialogueUI : MonoBehaviour
{
    public static CutsceneDialogueUI Instance;

    [Header("UI 요소")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerText;

    private Coroutine typingCoroutine;
    private Coroutine autoProgressCoroutine;

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
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    public void ShowDialogue(string speaker, string text, float typingSpeed, bool autoProgress, float autoDelay)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (autoProgressCoroutine != null) StopCoroutine(autoProgressCoroutine);

        speakerText.text = speaker;
        dialoguePanel.SetActive(true);

        typingCoroutine = StartCoroutine(TypeText(text, typingSpeed, autoProgress, autoDelay));
    }

    IEnumerator TypeText(string text, float speed, bool autoProgress, float autoDelay)
    {
        dialogueText.text = "";

        for (int i = 0; i < text.Length; i++)
        {
            dialogueText.text += text[i];

            float timer = 0f;
            while (timer < speed)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        // 타이핑 완료 후 자동 진행
        if (autoProgress)
        {
            autoProgressCoroutine = StartCoroutine(AutoHide(autoDelay));
        }
    }

    IEnumerator AutoHide(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        HideDialogue();
    }

    public void HideDialogue()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (autoProgressCoroutine != null) StopCoroutine(autoProgressCoroutine);

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    public bool IsActive()
    {
        return dialoguePanel != null && dialoguePanel.activeSelf;
    }
}