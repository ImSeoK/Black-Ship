using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    [Header("UI 요소")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerText;

    [Header("타이핑 효과")]
    public float typingSpeed = 0.05f;

    private string[] currentDialogues;
    private int currentIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (dialoguePanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                if (isTyping)
                {
                    // 타이핑 중이면 즉시 완성
                    CompleteText();
                }
                else
                {
                    // 다음 대사
                    ShowNextDialogue();
                }
            }
        }
    }

    public void ShowDialogue(string speaker, string[] dialogues)
    {
        speakerText.text = speaker;
        currentDialogues = dialogues;
        currentIndex = 0;

        dialoguePanel.SetActive(true);
        Time.timeScale = 0f; // 일시정지

        ShowNextDialogue();
    }

    void ShowNextDialogue()
    {
        if (currentIndex < currentDialogues.Length)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            typingCoroutine = StartCoroutine(TypeText(currentDialogues[currentIndex]));
            currentIndex++;
        }
        else
        {
            // 대화 종료
            CloseDialogue();
        }
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
    }

    void CompleteText()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        dialogueText.text = currentDialogues[currentIndex - 1];
        isTyping = false;
    }

    void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        Time.timeScale = 1f;
    }
}