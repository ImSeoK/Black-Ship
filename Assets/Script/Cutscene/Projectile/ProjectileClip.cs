using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class ProjectileClip : PlayableAsset, ITimelineClipAsset
{
    public ExposedReference<Projectile> projectile;

    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, UnityEngine.GameObject owner)
    {
        var playable = ScriptPlayable<ProjectileBehaviour>.Create(graph);
        playable.GetBehaviour().projectile = projectile.Resolve(graph.GetResolver());
        return playable;
    }
}