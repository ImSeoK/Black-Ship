using UnityEngine;
using UnityEngine.SceneManagement;

public class BabyManager : MonoBehaviour
{
    public static BabyManager Instance;

    // ���� �ñ� enum
    public enum BabyStage
    {
        Infant,     // ���Ʊ� - ���� ��� �ٴ�
        Toddler,    // �������� - ��������/�ΰ����� ����
        // ���� �ñ�� ��ȹ Ȯ�� �� �߰�
    }

    [Header("���� �ñ�")]
    public BabyStage currentStage = BabyStage.Infant;

    [Header("�ñ⺰ ������ ����")]
    public BabyData[] stageDataList;  // Inspector���� Infant, Toddler ������ �巡��

    [Header("�Ʊ� ��ġ/����")]
    public bool babyPickedUp = false;
    public bool carryingBaby = false;
    public string babySceneName = "";
    public Vector3 babyPosition;

    [Header("�Ʊ� �⺻ ����")]
    public string babyName = "�Ʊ�";
    public int babyAge = 0;

    [Header("�Ʊ� ����")]
    public BabyStats babyStats = new BabyStats();

    // ���߿� ���� �͵� (������ �ڸ���)
    // public BabyStats babyStats;
    // public BabyGrowthManager growthManager;

    static class SaveKeys
    {
        public const string PickedUp   = "BabyPickedUp";
        public const string Carrying   = "CarryingBaby";
        public const string SceneName  = "BabySceneName";
        public const string PosX       = "BabyPosX";
        public const string PosY       = "BabyPosY";
        public const string PosZ       = "BabyPosZ";
        public const string Name       = "BabyName";
        public const string Age        = "BabyAge";
        public const string Stage      = "BabyStage";
        public const string Health     = "BabyHealth";
        public const string Hunger     = "BabyHunger";
        public const string Happiness  = "BabyHappiness";
        public const string Affection  = "BabyAffection";
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadState();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ������ �ñ� üũ ������������������������������������������������������������������������������

    public bool IsInfant() => currentStage == BabyStage.Infant;

    // ���� �ñ� ������ ��������
    public BabyData GetCurrentStageData()
    {
        foreach (var data in stageDataList)
        {
            if (data.stage == currentStage)
                return data;
        }
        return null;
    }

    // ���� ���� �ƱⰡ �ִ��� üũ (BabyOnGround���� ���)
    public bool IsBabyInCurrentScene()
    {
        return babyPickedUp &&
               !carryingBaby &&
               babySceneName == SceneManager.GetActiveScene().name;
    }

    // ������ �Ʊ� ���� ���� ��������������������������������������������������������������������

    // �Ʊ� �߰� (�ƾ� ���� ������ ȣ��)
    public void PickUpBaby(bool carryImmediately = false)
    {
        babyPickedUp = true;
        carryingBaby = carryImmediately;
        currentStage = BabyStage.Infant;

        babyName = "���� �Ʊ�";
        babyAge = 0;

        // BabyStats �ʱ�ȭ
        babyStats.health = 100;
        babyStats.hunger = 50;
        babyStats.happiness = 80;
        babyStats.affection = 50;

        if (!carryImmediately)
        {
            babySceneName = "BaseCamp";
            babyPosition = new Vector3(3.5f, -1.5f, 0f);
        }

        SaveState();
    }

    // �Ʊ� ��� (BabyOnGround���� ȣ��)
    public void CarryBaby()
    {
        carryingBaby = true;
        babySceneName = "";
        SaveState();
    }

    // �Ʊ� �������� / �ΰ����� (BabyCarryController���� ȣ��)
    public void PutDownBaby(Vector3 position, string sceneName)
    {
        carryingBaby = false;
        babyPosition = position;
        babySceneName = sceneName;
        SaveState();
    }

    // �ñ� ���� (���߿� BabyGrowthManager�� ȣ��)
    public void GrowToNextStage()
    {
        if (currentStage < BabyStage.Toddler)
        {
            currentStage++;
            Debug.Log($"�Ʊ� ����! ���� �ñ�: {currentStage}");
            SaveState();
        }
    }

    // ������ ���� / �ε� ��������������������������������������������������������������������������
    public void SaveState()
    {
        PlayerPrefs.SetInt(SaveKeys.PickedUp,  babyPickedUp ? 1 : 0);
        PlayerPrefs.SetInt(SaveKeys.Carrying,  carryingBaby ? 1 : 0);
        PlayerPrefs.SetString(SaveKeys.SceneName, babySceneName);
        PlayerPrefs.SetFloat(SaveKeys.PosX,    babyPosition.x);
        PlayerPrefs.SetFloat(SaveKeys.PosY,    babyPosition.y);
        PlayerPrefs.SetFloat(SaveKeys.PosZ,    babyPosition.z);
        PlayerPrefs.SetString(SaveKeys.Name,   babyName);
        PlayerPrefs.SetInt(SaveKeys.Age,       babyAge);
        PlayerPrefs.SetInt(SaveKeys.Stage,     (int)currentStage);
        PlayerPrefs.SetInt(SaveKeys.Health,    babyStats.health);
        PlayerPrefs.SetInt(SaveKeys.Hunger,    babyStats.hunger);
        PlayerPrefs.SetInt(SaveKeys.Happiness, babyStats.happiness);
        PlayerPrefs.SetInt(SaveKeys.Affection, babyStats.affection);
        PlayerPrefs.Save();
    }

    public void LoadState()
    {
        babyPickedUp  = PlayerPrefs.GetInt(SaveKeys.PickedUp,  0) == 1;
        carryingBaby  = PlayerPrefs.GetInt(SaveKeys.Carrying,  0) == 1;
        babySceneName = PlayerPrefs.GetString(SaveKeys.SceneName, "");
        babyPosition.x = PlayerPrefs.GetFloat(SaveKeys.PosX,  0);
        babyPosition.y = PlayerPrefs.GetFloat(SaveKeys.PosY,  0);
        babyPosition.z = PlayerPrefs.GetFloat(SaveKeys.PosZ,  0);
        babyName      = PlayerPrefs.GetString(SaveKeys.Name,  "아기");
        babyAge       = PlayerPrefs.GetInt(SaveKeys.Age,       0);
        currentStage  = (BabyStage)PlayerPrefs.GetInt(SaveKeys.Stage, 0);
        babyStats.health    = PlayerPrefs.GetInt(SaveKeys.Health,    100);
        babyStats.hunger    = PlayerPrefs.GetInt(SaveKeys.Hunger,    50);
        babyStats.happiness = PlayerPrefs.GetInt(SaveKeys.Happiness, 80);
        babyStats.affection = PlayerPrefs.GetInt(SaveKeys.Affection, 50);
    }
}