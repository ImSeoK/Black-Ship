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
                if (LoadingManager.Instance != null)
                    LoadingManager.Instance.StartCoroutine(LoadingManager.Instance.Fade(0f, 1f));
                break;

            case EffectClip.EffectType.FadeIn:
                if (LoadingManager.Instance != null)
                    LoadingManager.Instance.StartCoroutine(LoadingManager.Instance.Fade(1f, 0f));
                break;
        }
    }
}