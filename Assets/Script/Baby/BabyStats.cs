using UnityEngine;

[System.Serializable]
public class BabyStats
{
    [Header("기본 수치")]
    public int health = 100;       // 건강
    public int hunger = 50;        // 배고픔 (0~100, 높을수록 배고픔)
    public int happiness = 80;     // 행복도

    [Header("호감도 / 성격")]
    public int affection = 50;     // 호감도 (0~100)
                                   // 높으면 적극적, 낮으면 소극적

    // 성격 방향 반환 (BabySkillHandler에서 사용)
    // 50 기준으로 위아래로 나뉨
    public float GetPersonalityRatio()
    {
        return affection / 100f;   // 0.0 ~ 1.0
    }

    // 호감도 변경 (범위 보정 포함)
    public void AddAffection(int amount)
    {
        affection = Mathf.Clamp(affection + amount, 0, 100);
    }

    public void AddHunger(int amount)
    {
        hunger = Mathf.Clamp(hunger + amount, 0, 100);
    }

    public void AddHappiness(int amount)
    {
        happiness = Mathf.Clamp(happiness + amount, 0, 100);
    }
}