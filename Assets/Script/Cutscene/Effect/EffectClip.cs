using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class EffectClip : PlayableAsset, ITimelineClipAsset
{
    public enum EffectType { PlaySound, FadeOut, FadeIn, FadeInWhite, FadeOutWhite }

    [Header("이펙트 설정")]
    public EffectType effectType = EffectType.FadeIn;

    [Header("사운드")]
    public AudioClip soundClip;

    [Header("카메라 흔들림")]
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.3f;

    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<EffectBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.effectType = effectType;
        behaviour.soundClip = soundClip;
        behaviour.shakeDuration = shakeDuration;
        behaviour.shakeMagnitude = shakeMagnitude;
        return playable;
    }
}