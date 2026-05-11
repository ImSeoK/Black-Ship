using UnityEngine;

public class ConditionalObject : MonoBehaviour
{
    public enum ConditionType
    {
        BabyPickedUp,
        CarryingBaby,
        ForestCutscenePlayed
    }

    public ConditionType condition;
    public bool showWhenTrue = false;

    void Start()
    {
        CheckCondition();
    }

    void CheckCondition()
    {
        bool conditionMet = false;

        switch (condition)
        {
            case ConditionType.BabyPickedUp:
                if (BabyManager.Instance != null)
                    conditionMet = BabyManager.Instance.babyPickedUp;
                break;
            case ConditionType.CarryingBaby:
                if (BabyManager.Instance != null)
                    conditionMet = BabyManager.Instance.carryingBaby;
                break;
            case ConditionType.ForestCutscenePlayed:
                conditionMet = PlayerPrefs.GetInt("Cutscene_ForestCutscene", 0) == 1;
                break;
        }

        gameObject.SetActive(showWhenTrue ? conditionMet : !conditionMet);
    }
}