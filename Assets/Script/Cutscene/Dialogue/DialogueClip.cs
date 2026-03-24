using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class DialogueClip : PlayableAsset, ITimelineClipAsset
{
    [Header("��� ����")]
    public string speaker = "???";

    [TextArea(3, 10)]
    public string dialogueText = "";

    [Header("Ÿ���� ����")]
    [Range(0.01f, 0.2f)]
    public float typingSpeed = 0.05f;
    public bool autoProgress = true;
    public float autoProgressDelay = 2f;

    public ClipCaps clipCaps => ClipCaps.None;

    // 타이핑 완료 시간 기반 자동 클립 길이 계산
    // 글자수 × typingSpeed + (autoProgress이면 autoProgressDelay, 아니면 여유 0.5초)
    public override double duration
    {
        get
        {
            double typingTime = dialogueText.Length * typingSpeed;
            double holdTime  = autoProgress ? autoProgressDelay : 0.5;
            return System.Math.Max(typingTime + holdTime, 0.1);
        }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DialogueBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();

        behaviour.speaker = speaker;
        behaviour.dialogueText = dialogueText;
        behaviour.typingSpeed = typingSpeed;
        behaviour.autoProgress = autoProgress;
        behaviour.autoProgressDelay = autoProgressDelay;

        return playable;
    }
}