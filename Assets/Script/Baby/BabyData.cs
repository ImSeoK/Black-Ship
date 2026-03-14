using UnityEngine;

[CreateAssetMenu(fileName = "BabyStageData", menuName = "Baby/Stage Data")]
public class BabyData : ScriptableObject
{
    [Header("시기 정보")]
    public string stageName = "";           // 유아기, 걸음마기 등
    public BabyManager.BabyStage stage;    // 어떤 시기인지

    [Header("성장 조건 (추후 기획 확정 후 채움)")]
    public int growthAffectionRequired = 0; // 필요 호감도
    // 스테이지 기반 or 시간 기반은 나중에 추가

    [Header("상태이상 (추후 기획 확정 후 채움)")]
    public string[] possibleStatusEffects;  // 이 시기에 가능한 상태이상 목록

    [Header("이동 패턴")]
    public bool canBeCarried = true;        // 들 수 있는지 (유아기만 true)
}