using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class ProjectileBehaviour : PlayableBehaviour
{
    private bool hasExecuted = false;

    public override void OnGraphStart(Playable playable)
    {
        hasExecuted = false;
    }

    public Projectile projectile;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (hasExecuted) return;
        if (!Application.isPlaying) return;
        hasExecuted = true;

        projectile?.Fire();
    }
}