using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.2f, 0.8f, 1f)]
[TrackClipType(typeof(DialogueClip))]
public class DialogueTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<DialogueTrackMixer>.Create(graph, inputCount);
    }
}

public class DialogueTrackMixer : PlayableBehaviour { }