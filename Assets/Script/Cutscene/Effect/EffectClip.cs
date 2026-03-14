using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class EffectClip : PlayableAsset, ITimelineClipAsset
{
    public enum EffectType { CameraShake, PlaySound, FadeOut, FadeIn }

    [Header("이펙트 설정")]
    public EffectType effectType = EffectType.CameraShake;

    [Header("카메라 흔들림")]
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.5f;

    [Header("사운드")]
    public AudioClip soundClip;

    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<EffectBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();

        behaviour.effectType = effectType;
        behaviour.shakeDuration = shakeDuration;
        behaviour.shakeMagnitude = shakeMagnitude;
        behaviour.soundClip = soundClip;

        return playable;
    }
}