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

        float currentTime = (float)playable.GetTime();
        float duration = (float)playable.GetDuration();

        switch (effectType)
        {
            case EffectClip.EffectType.FadeIn:
                ScreenFadeUI.Instance?.SetFadeColor(Color.black);
                ScreenFadeUI.Instance?.SetAlpha(1f - (currentTime / duration));
                break;

            case EffectClip.EffectType.FadeOut:
                ScreenFadeUI.Instance?.SetFadeColor(Color.black);
                ScreenFadeUI.Instance?.SetAlpha(currentTime / duration);
                break;

            case EffectClip.EffectType.FadeInWhite:
                ScreenFadeUI.Instance?.SetFadeColor(Color.white);
                ScreenFadeUI.Instance?.SetAlpha(1f - (currentTime / duration));
                break;

            case EffectClip.EffectType.FadeOutWhite:
                ScreenFadeUI.Instance?.SetFadeColor(Color.white);
                ScreenFadeUI.Instance?.SetAlpha(currentTime / duration);
                break;
        }
    }
}