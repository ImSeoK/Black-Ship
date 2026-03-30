using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    [Header("UI 참조")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    [Tooltip("타이핑 완료 후 표시할 진행 표시(다음 버튼 화살표 등)")]
    public GameObject nextIndicator;

    [Header("설정")]
    public float typingSpeed = 0.05f;

    private string[] dialogues;
    private int index = 0;
    private bool isTyping = false;
    private string currentFullText = "";
    private Coroutine typingCoroutine;

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
        dialoguePanel.SetActive(false);
        nextIndicator?.SetActive(false);
    }

    public void ShowDialogue(string speaker, string[] texts, SpeakerType speakerType = SpeakerType.Other)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        dialogues = (string[])texts.Clone();
        index = 0;

        speakerText.text = speaker;
        dialogueText.color = SpeakerColors.GetColor(speakerType);

        dialoguePanel.SetActive(true);
        nextIndicator?.SetActive(false);
        Time.timeScale = 0f;

        typingCoroutine = StartCoroutine(Type());
    }

    IEnumerator Type()
    {
        isTyping = true;
        nextIndicator?.SetActive(false);

        currentFullText = dialogues[index];
        dialogueText.text = "";

        foreach (char c in currentFullText)
        {
            dialogueText.text += c;

            float timer = 0f;
            while (timer < typingSpeed)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        isTyping = false;
        typingCoroutine = null;
        nextIndicator?.SetActive(true);
    }

    void InstantComplete()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        dialogueText.text = currentFullText;
        isTyping = false;
        nextIndicator?.SetActive(true);
    }

    void Update()
    {
        if (!dialoguePanel.activeSelf) return;
        if (!Input.GetKeyDown(KeyCode.Space)) return;

        if (isTyping)
        {
            // 타이핑 중 → 즉시 완성
            InstantComplete();
            return;
        }

        // 타이핑 완료 상태 → 다음 대사 또는 종료
        nextIndicator?.SetActive(false);
        index++;

        if (index < dialogues.Length)
        {
            typingCoroutine = StartCoroutine(Type());
        }
        else
        {
            dialoguePanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public bool IsDialogueActive()
    {
        return dialoguePanel != null && dialoguePanel.activeSelf;
    }
}
