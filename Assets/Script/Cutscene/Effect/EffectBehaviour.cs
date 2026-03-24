using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class EffectBehaviour : PlayableBehaviour
{
    // LoadingManager를 직접 참조하지 않고 이벤트로 요청
    // LoadingManager(또는 다른 구독자)가 처리
    public static System.Action<float, float> OnFadeRequested;

    public EffectClip.EffectType effectType;
    public float shakeDuration;
    public float shakeMagnitude;
    public AudioClip soundClip;

    private bool hasExecuted = false;

    public override void OnGraphStart(Playable playable)
    {
        hasExecuted = false;
    }

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

            case EffectClip.EffectType.FadeOut:
                OnFadeRequested?.Invoke(0f, 1f);
                break;

            case EffectClip.EffectType.FadeIn:
                OnFadeRequested?.Invoke(1f, 0f);
                break;
        }
    }
}
