using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(menuName = "Cutscene/CutsceneData")]
public class CutsceneData : ScriptableObject
{
    [Header("ÄÆ¾À ¼³Á¤")]
    public string cutsceneID;
    public PlayableDirector timeline;
    public bool playOnce = true;
}