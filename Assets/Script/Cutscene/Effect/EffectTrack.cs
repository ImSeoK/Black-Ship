using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// Timeline에 추가되는 커스텀 효과 트랙
/// EffectClip을 배치할 수 있음
/// </summary>
[TrackColor(1f, 0.5f, 0.2f)] // 주황색
[TrackClipType(typeof(EffectClip))]
public class EffectTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<EffectTrackMixer>.Create(graph, inputCount);
    }
}

public class EffectTrackMixer : PlayableBehaviour
{
    // 특별한 믹싱 로직 불필요
}

/// <summary>
/// 효과 Clip의 실제 실행 로직
/// </summary>
[System.Serializable]
public class EffectBehaviour : PlayableBehaviour
{
    public EffectClip.EffectType effectType;
    public float shakeDuration;
    public float shakeMagnitude;
    public AudioClip soundClip;

    private bool hasExecuted = false;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (hasExecuted) return;
        hasExecuted = true;

        switch (effectType)
        {
            case EffectClip.EffectType.CameraShake:
                ExecuteCameraShake();
                break;

            case EffectClip.EffectType.PlaySound:
                ExecutePlaySound();
                break;

            case EffectClip.EffectType.FadeOut:
                // TODO: 페이드 아웃 구현
                break;

            case EffectClip.EffectType.FadeIn:
                // TODO: 페이드 인 구현
                break;
        }
    }

    void ExecuteCameraShake()
    {
        if (CinemachineShaker.Instance != null)
        {
            CinemachineShaker.Instance.Shake(shakeDuration, shakeMagnitude);
        }
        else
        {
            Debug.LogWarning("CinemachineShaker.Instance가 null입니다!");
        }
    }

    void ExecutePlaySound()
    {
        if (SoundManager.Instance != null && soundClip != null)
        {
            SoundManager.Instance.PlaySound(soundClip);
        }
        else
        {
            Debug.LogWarning($"사운드 재생 실패 - SoundManager: {SoundManager.Instance != null}, Clip: {soundClip != null}");
        }
    }
}