using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// 대사 Clip의 실제 재생 로직
/// Timeline 재생 중 자동으로 호출됨
/// </summary>
[System.Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
    public string speaker;
    public string dialogueText;
    public float typingSpeed;
    public bool autoProgress;
    public float autoProgressDelay;

    private bool hasStarted = false;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (hasStarted) return;
        hasStarted = true;

        if (CutsceneDialogueUI.Instance != null)
        {
            CutsceneDialogueUI.Instance.ShowDialogue(
                speaker,
                dialogueText,
                typingSpeed,
                autoProgress,
                autoProgressDelay
            );
        }
        else
        {
            Debug.LogError("CutsceneDialogueUI.Instance가 null입니다!");
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (CutsceneDialogueUI.Instance != null)
        {
            CutsceneDialogueUI.Instance.HideDialogue();
        }
    }
}