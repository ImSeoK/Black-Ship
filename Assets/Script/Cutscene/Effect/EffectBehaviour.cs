using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class EffectBehaviour : PlayableBehaviour
{
    public EffectClip.EffectType effectType;
    public float shakeDuration;
    public float shakeMagnitude;
    public AudioClip soundClip;

    private bool hasExecuted = false;

    public override void OnGraphStart(Playable playable)
    {
        hasExecuted = false;
    }

    // CameraShake, PlaySound 등 일회성 효과만 여기서 처리
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (hasExecuted) return;
        if (!Application.isPlaying) return;
        hasExecuted = true;

        switch (effectType)
        {
            case EffectClip.EffectType.CameraShake:
                CameraShake.Instance?.Shake(shakeDuration, shakeMagnitude);
                break;

            case EffectClip.EffectType.PlaySound:
                if (soundClip != null)
                    AudioSource.PlayClipAtPoint(soundClip, Camera.main.transform.position);
                break;
        }
    }

    // FadeIn / FadeOut은 매 프레임 타임라인 진행도에 맞춰 알파를 직접 구동
    // → 타임라인의 다른 트랙(플레이어 애니메이션 등)과 완벽하게 동기화됨
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (!Application.isPlaying) return;
        if (effectType != EffectClip.EffectType.FadeIn && effectType != EffectClip.EffectType.FadeOut) return;

        float currentTime = (float)playable.GetTime();
        float duration = (float)playable.GetDuration();

        float alpha = effectType == EffectClip.EffectType.FadeIn
            ? 1f - (currentTime / duration)
            : currentTime / duration;

        ScreenFadeUI.Instance?.SetAlpha(alpha);
    }
}
