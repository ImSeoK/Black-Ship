using UnityEngine;

[System.Serializable]
public class BabyData
{
    public string babyName = "아기";
    public int age = 0;           // 개월 수
    public int health = 100;      // 건강
    public int hunger = 50;       // 배고픔 (0~100, 높을수록 배고픔)
    public int happiness = 80;    // 행복도
}