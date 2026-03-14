using UnityEngine;
using TMPro;
using System.Collections;

public class CutsceneDialogueUI : MonoBehaviour
{
    public static CutsceneDialogueUI Instance;

    [Header("UI ÂüÁ¶")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerText;

    private Coroutine typingCoroutine;
    private Coroutine autoHideCoroutine;

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

        speakerText.text = speaker;
        dialoguePanel.SetActive(true);
        typingCoroutine = StartCoroutine(TypeText(text, typingSpeed, autoProgress, autoDelay));
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