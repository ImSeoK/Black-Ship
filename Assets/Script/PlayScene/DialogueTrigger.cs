using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("화자 설정")]
    public string speakerName = "???";
    public SpeakerType speakerType = SpeakerType.Other;

    [Header("대사 목록")]
    [TextArea(3, 10)]
    public string[] dialogues;

    public void TriggerDialogue()
    {
        if (DialogueUI.Instance != null && dialogues.Length > 0)
        {
            DialogueUI.Instance.ShowDialogue(speakerName, dialogues, speakerType);
        }
    }
}
