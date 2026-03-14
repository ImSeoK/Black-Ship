using UnityEngine;

public class BabyGrowthManager : MonoBehaviour
{
    public static BabyGrowthManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ─── 성장 조건 체크 ──────────────────────────────────
    // 나중에 기획 확정되면 여기 조건 채우면 됨
    // 지금은 외부에서 직접 호출하는 방식으로만 동작

    // 스테이지 기반 성장 (특정 이벤트/보스 처치 후 호출)
    public void TriggerGrowth()
    {
        if (BabyManager.Instance == null) return;

        BabyManager.BabyStage currentStage = BabyManager.Instance.currentStage;

        // 이미 최대 시기면 무시
        if ((int)currentStage >= System.Enum.GetValues(typeof(BabyManager.BabyStage)).Length - 1)
        {
            Debug.Log("이미 최대 성장 단계입니다.");
            return;
        }

        BabyManager.Instance.GrowToNextStage();
        OnGrowth(BabyManager.Instance.currentStage);
    }

    // 성장 시 처리 (연출, 이벤트 등 나중에 추가)
    void OnGrowth(BabyManager.BabyStage newStage)
    {
        Debug.Log($"아기가 성장했습니다! → {newStage}");

        // TODO: 성장 연출 (컷씬, 이펙트 등)
        // TODO: 성격 확정 로직
        // TODO: 스킬 해금
    }

    // ─── 조건 기반 체크 (추후 기획 확정 후 채움) ─────────
    // 예시: 호감도 기반
    // void CheckAffectionGrowth()
    // {
    //     if (BabyManager.Instance.babyStats.affection >= 80)
    //         TriggerGrowth();
    // }

    // 예시: 인게임 시간 기반
    // void CheckTimeGrowth(float elapsedTime)
    // {
    //     if (elapsedTime >= growthTimeThreshold)
    //         TriggerGrowth();
    // }
}