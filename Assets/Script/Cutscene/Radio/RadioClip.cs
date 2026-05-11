using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class RadioClip : PlayableAsset, ITimelineClipAsset
{
    public string caller;
    [TextArea] public string message;
    public bool autoHide = true;

    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<RadioBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.caller = caller;
        behaviour.message = message;
        behaviour.autoHide = autoHide;
        return playable;
    }
}