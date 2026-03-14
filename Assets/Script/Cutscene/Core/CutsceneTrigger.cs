using UnityEngine;
using UnityEngine.Playables;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("ÄÆ¾À ¼³Á¤")]
    public PlayableDirector director;
    public string cutsceneID;
    public bool playOnce = true;

    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (hasTriggered) return;

        hasTriggered = true;
        CutsceneManager.Instance?.Play(director, cutsceneID, playOnce);
    }
}