using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// Timeline에 추가되는 커스텀 대사 트랙
/// DialogueClip을 배치할 수 있음
/// </summary>
[TrackColor(0.2f, 0.8f, 1f)] // 파란색
[TrackClipType(typeof(DialogueClip))]
[TrackBindingType(typeof(GameObject))] // 바인딩 불필요하지만 형식상 추가
public class DialogueTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<DialogueTrackMixer>.Create(graph, inputCount);
    }
}

/// <summary>
/// 여러 Clip을 믹싱 (현재는 단순히 통과)
/// </summary>
public class DialogueTrackMixer : PlayableBehaviour
{
    // 특별한 믹싱 로직 불필요
    // 각 Clip이 개별적으로 재생됨
}