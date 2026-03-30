using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(menuName = "Cutscene/CutsceneData")]
public class CutsceneData : ScriptableObject
{
    [Header("컷씬 설정")]
    public string cutsceneID;
    public PlayableAsset timelineAsset;  // Timeline .asset 파일 (ScriptableObject)
    public bool playOnce = true;
    [Tooltip("true면 컷씬 종료 시 레터박스를 유지합니다 (연속 컷씬 전환 시 슬라이드 없이 이어붙이기)")]
    public bool keepLetterboxOnEnd = false;
}
