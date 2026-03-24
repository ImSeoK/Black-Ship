using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerText;
    public float typingSpeed = 0.05f;

    private string[] dialogues;
    private int index = 0;

    private bool canAcceptInput = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ← 이것만 추가
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

    public void ShowDialogue(string speaker, string[] texts)
    {
        StopAllCoroutines();
        dialogues = (string[])texts.Clone();
        index = 0;
        speakerText.text = speaker;
        dialoguePanel.SetActive(true);
        Time.timeScale = 0f;

        canAcceptInput = false; // 입력 막기
        StartCoroutine(Type());
    }

    IEnumerator Type()
    {
        string fullText = dialogues[index];
        dialogueText.text = "";

        for (int i = 0; i < fullText.Length; i++)
        {
            dialogueText.text += fullText[i];

            float timer = 0f;
            while (timer < typingSpeed)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        canAcceptInput = true; // 타이핑 끝나면 입력 허용
    }

    void Update()
    {
        if (!dialoguePanel.activeSelf) return;

        if (canAcceptInput && Input.GetKeyDown(KeyCode.Space))
        {
            canAcceptInput = false; // 중복 방지
            index++;
            if (index < dialogues.Length)
            {
                StopAllCoroutines();
                StartCoroutine(Type());
            }
            else
            {
                dialoguePanel.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }

    public bool IsDialogueActive()
    {
        return dialoguePanel != null && dialoguePanel.activeSelf;
    }
}