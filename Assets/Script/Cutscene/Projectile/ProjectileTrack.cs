using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackBindingType(typeof(Projectile))]
[TrackColor(0.8f, 0.2f, 0.2f)]
[TrackClipType(typeof(ProjectileClip))]
public class ProjectileTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, UnityEngine.GameObject go, int inputCount)
    {
        return ScriptPlayable<ProjectileTrackMixer>.Create(graph, inputCount);
    }
}

public class ProjectileTrackMixer : PlayableBehaviour { }