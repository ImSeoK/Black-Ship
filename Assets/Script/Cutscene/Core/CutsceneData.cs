using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(menuName = "Cutscene/CutsceneData")]
public class CutsceneData : ScriptableObject
{
    [Header("컷씬 설정")]
    public string cutsceneID;
    public PlayableAsset timelineAsset;  // Timeline .asset 파일 (ScriptableObject)
    public bool playOnce = true;
}
