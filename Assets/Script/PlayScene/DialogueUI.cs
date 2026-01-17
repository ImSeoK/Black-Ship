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
        // 코루틴 중단
        StopAllCoroutines();

        // 배열 새로 복사
        dialogues = new string[texts.Length];
        for (int i = 0; i < texts.Length; i++)
        {
            dialogues[i] = texts[i];
        }

        index = 0;
        speakerText.text = speaker;
        dialoguePanel.SetActive(true);
        Time.timeScale = 0f;

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
    }

    void Update()
    {
        if (!dialoguePanel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
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
        return dialoguePanel.activeSelf;
    }
}