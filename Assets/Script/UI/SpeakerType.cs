using UnityEngine;

/// <summary>
/// 대화 화자 유형. DialogueUI와 CutsceneDialogueUI에서 공용으로 사용합니다.
/// </summary>
public enum SpeakerType
{
    Other,       // 주인공이 아닌 자  #c8a84a
    Protagonist, // 주인공            #8ab8c8
    Narration    // 나레이션          #888860
}

/// <summary>
/// SpeakerType별 텍스트 색상을 제공합니다.
/// </summary>
public static class SpeakerColors
{
    public static readonly Color Other       = HexToColor("c8a84a");
    public static readonly Color Protagonist = HexToColor("8ab8c8");
    public static readonly Color Narration   = HexToColor("888860");

    public static Color GetColor(SpeakerType type)
    {
        switch (type)
        {
            case SpeakerType.Protagonist: return Protagonist;
            case SpeakerType.Narration:   return Narration;
            default:                      return Other;
        }
    }

    static Color HexToColor(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }
}
