using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// Timeline에 배치되는 효과 Clip
/// 카메라 쉐이크, 사운드 재생 등
/// </summary>
[System.Serializable]
public class EffectClip : PlayableAsset, ITimelineClipAsset
{
    public enum EffectType
    {
        CameraShake,
        PlaySound,
        FadeOut,
        FadeIn
    }

    [Header("효과 설정")]
    public EffectType effectType = EffectType.CameraShake;

    [Header("카메라 쉐이크")]
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.5f;

    [Header("사운드")]
    public AudioClip soundClip;

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<EffectBehaviour>.Create(graph);
        EffectBehaviour behaviour = playable.GetBehaviour();

        behaviour.effectType = effectType;
        behaviour.shakeDuration = shakeDuration;
        behaviour.shakeMagnitude = shakeMagnitude;
        behaviour.soundClip = soundClip;

        return playable;
    }
}