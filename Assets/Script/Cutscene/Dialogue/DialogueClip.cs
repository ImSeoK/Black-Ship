using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class DialogueClip : PlayableAsset, ITimelineClipAsset
{
    [Header("화자 설정")]
    public string speaker = "???";
    public SpeakerType speakerType = SpeakerType.Other;

    [Header("대사")]
    [TextArea(3, 10)]
    public string dialogueText = "";

    [Header("타이핑 속도")]
    [Range(0.01f, 0.2f)]
    public float typingSpeed = 0.05f;

    public ClipCaps clipCaps => ClipCaps.None;

    // 플레이어가 직접 진행하므로 클립 길이는 최솟값으로 고정.
    // Timeline에서는 클립이 시작되자마자 director가 Pause되므로
    // 실제 대기 시간은 클립 길이와 무관합니다.
    public override double duration => 0.5;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DialogueBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();

        behaviour.speaker      = speaker;
        behaviour.dialogueText = dialogueText;
        behaviour.typingSpeed  = typingSpeed;
        behaviour.speakerType  = speakerType;

        return playable;
    }
}
