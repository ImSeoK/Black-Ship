using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class EffectBehaviour : PlayableBehaviour
{
    public EffectClip.EffectType effectType;
    public AudioClip soundClip;
    public float shakeDuration;
    public float shakeMagnitude;

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
            case EffectClip.EffectType.PlaySound:
                if (soundClip != null)
                    AudioSource.PlayClipAtPoint(soundClip, Camera.main.transform.position);
                break;
        }
    }

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