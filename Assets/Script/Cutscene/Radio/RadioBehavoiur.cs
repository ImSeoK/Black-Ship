using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class RadioBehaviour : PlayableBehaviour
{
    public string caller;
    public string message;
    public bool autoHide = true;

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

        RadioTutorialUI.Instance?.ShowRadioMessage(caller, message, autoHide);
    }
}