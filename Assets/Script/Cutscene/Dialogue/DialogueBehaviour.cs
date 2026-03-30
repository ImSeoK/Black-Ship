using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
    public string speaker;
    public string dialogueText;
    public float typingSpeed;
    public SpeakerType speakerType;

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

        if (CutsceneDialogueUI.Instance == null) return;

        // graph resolver를 통해 PlayableDirector 참조 획득
        var director = playable.GetGraph().GetResolver() as PlayableDirector;

        // 현재 클립의 타임라인 상 종료 시점 계산
        double clipEndTime = director != null
            ? director.time + (playable.GetDuration() - playable.GetTime())
            : 0.0;

        CutsceneDialogueUI.Instance.ShowDialogue(
            speaker, dialogueText, typingSpeed, speakerType, director, clipEndTime
        );
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (!Application.isPlaying) return;
        if (CutsceneDialogueUI.Instance == null) return;

        // SetSpeed(0) 방식으로 동결하므로 OnBehaviourPause는 클립 자연 종료 시에만 발화됨
        // 이 시점에 HideDialogue()는 이미 spacebar 입력으로 호출된 상태이므로 무해하게 처리됨
        CutsceneDialogueUI.Instance.HideDialogue();
    }
}
