using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
    public string speaker;
    public string dialogueText;
    public float typingSpeed;
    public bool autoProgress;
    public float autoProgressDelay;

    private bool hasStarted = false;

    public override void OnGraphStart(Playable playable)
    {
        hasStarted = false;
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (hasStarted) return;
        if (!Application.isPlaying) return;
        hasStarted = true;

        if (CutsceneDialogueUI.Instance != null)
            CutsceneDialogueUI.Instance.ShowDialogue(
                speaker, dialogueText, typingSpeed, autoProgress, autoProgressDelay
            );
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (!Application.isPlaying) return;
        if (CutsceneDialogueUI.Instance != null)
            CutsceneDialogueUI.Instance.HideDialogue();
    }
}