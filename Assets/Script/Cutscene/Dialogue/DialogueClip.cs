using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class DialogueClip : PlayableAsset, ITimelineClipAsset
{
    [Header("대사 설정")]
    public string speaker = "???";

    [TextArea(3, 10)]
    public string dialogueText = "";

    [Header("타이핑 설정")]
    [Range(0.01f, 0.2f)]
    public float typingSpeed = 0.05f;
    public bool autoProgress = true;
    public float autoProgressDelay = 2f;

    public ClipCaps clipCaps => ClipCaps.None;

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