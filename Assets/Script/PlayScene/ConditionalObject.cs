using UnityEngine;

public class ConditionalObject : MonoBehaviour
{
    public enum ConditionType
    {
        BabyPickedUp,
        CarryingBaby,
        ForestCutscenePlayed
    }

    [Header("조건 설정")]
    public ConditionType condition;
    public bool showWhenTrue = false;

    void Start()
    {
        CheckCondition();
    }

    void CheckCondition()
    {
        if (StatsManager.Instance == null) return;

        bool conditionMet = false;

        switch (condition)
        {
            case ConditionType.BabyPickedUp:
                conditionMet = StatsManager.Instance.babyPickedUp;
                break;
            case ConditionType.CarryingBaby:
                conditionMet = StatsManager.Instance.carryingBaby;
                break;
            case ConditionType.ForestCutscenePlayed:
                conditionMet = StatsManager.Instance.forestCutscenePlayed;
                break;
        }

        // 조건에 따라 활성화/비활성화
        if (showWhenTrue)
        {
            gameObject.SetActive(conditionMet);
        }
        else
        {
            gameObject.SetActive(!conditionMet);
        }
    }
}