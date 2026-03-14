using UnityEngine;

public class BabyTestHelper : MonoBehaviour
{
    void Update()
    {
        // F1: 아기 발견
        if (Input.GetKeyDown(KeyCode.F1))
        {
            BabyManager.Instance.PickUpBaby(true);
            Debug.Log("아기 발견!");
        }

        // F2: 호감도 +20
        if (Input.GetKeyDown(KeyCode.F2))
        {
            BabyManager.Instance.babyStats.AddAffection(20);
            BabySkillHandler.Instance.UpdatePersonality();
            Debug.Log($"호감도: {BabyManager.Instance.babyStats.affection}");
        }

        // F3: 호감도 -20
        if (Input.GetKeyDown(KeyCode.F3))
        {
            BabyManager.Instance.babyStats.AddAffection(-20);
            BabySkillHandler.Instance.UpdatePersonality();
            Debug.Log($"호감도: {BabyManager.Instance.babyStats.affection}");
        }

        // F4: 성장
        if (Input.GetKeyDown(KeyCode.F4))
        {
            BabyGrowthManager.Instance.TriggerGrowth();
        }

        // F5: 현재 상태 출력
        if (Input.GetKeyDown(KeyCode.F5))
        {
            var bm = BabyManager.Instance;
            Debug.Log($"시기: {bm.currentStage} | 호감도: {bm.babyStats.affection} | 성격: {BabySkillHandler.Instance.currentPersonality} | 들고있음: {bm.carryingBaby}");
        }
    }
}