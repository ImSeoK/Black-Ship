using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// Timeline에 배치되는 대사 Clip
/// Inspector에서 직접 편집 가능
/// </summary>
[System.Serializable]
public class DialogueClip : PlayableAsset, ITimelineClipAsset
{
    [Header("대사 설정")]
    public string speaker = "주인공";

    [TextArea(3, 10)]
    public string dialogueText = "";

    [Header("타이밍 설정")]
    [Range(0.01f, 0.2f)]
    public float typingSpeed = 0.05f;

    public bool autoProgress = true;

    [Tooltip("자동 진행 시 타이핑 완료 후 대기 시간")]
    public float autoProgressDelay = 2f;

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DialogueBehaviour>.Create(graph);
        DialogueBehaviour behaviour = playable.GetBehaviour();

        behaviour.speaker = speaker;
        behaviour.dialogueText = dialogueText;
        behaviour.typingSpeed = typingSpeed;
        behaviour.autoProgress = autoProgress;
        behaviour.autoProgressDelay = autoProgressDelay;

        return playable;
    }
}