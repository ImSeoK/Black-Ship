using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("화자 정보")]
    public string speakerName = "???";

    [Header("대사 내용")]
    [TextArea(3, 10)]
    public string[] dialogues;

    public void TriggerDialogue()
    {
        if (DialogueUI.Instance != null && dialogues.Length > 0)
        {
            DialogueUI.Instance.ShowDialogue(speakerName, dialogues);
        }
    }
}