using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(1f, 0.5f, 0.2f)]
[TrackClipType(typeof(EffectClip))]
public class EffectTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<EffectTrackMixer>.Create(graph, inputCount);
    }
}

public class EffectTrackMixer : PlayableBehaviour { }