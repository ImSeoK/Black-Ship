using UnityEngine;
using UnityEngine.SceneManagement;

public class BabyManager : MonoBehaviour
{
    public static BabyManager Instance;

    // 성장 시기 enum
    public enum BabyStage
    {
        Infant,     // 유아기 - 직접 들고 다님
        Toddler,    // 걸음마기 - 데려가기/두고가기 선택
        // 이후 시기는 기획 확정 후 추가
    }

    [Header("성장 시기")]
    public BabyStage currentStage = BabyStage.Infant;

    [Header("시기별 데이터 에셋")]
    public BabyData[] stageDataList;  // Inspector에서 Infant, Toddler 순서로 드래그

    [Header("아기 위치/상태")]
    public bool babyPickedUp = false;
    public bool carryingBaby = false;
    public string babySceneName = "";
    public Vector3 babyPosition;

    [Header("아기 기본 스탯")]
    public string babyName = "아기";
    public int babyAge = 0;

    [Header("아기 스탯")]
    public BabyStats babyStats = new BabyStats();

    // 나중에 붙을 것들 (지금은 자리만)
    // public BabyStats babyStats;
    // public BabyGrowthManager growthManager;

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

    // ─── 시기 체크 ───────────────────────────────────────

    public bool IsInfant() => currentStage == BabyStage.Infant;

    // 현재 시기 데이터 가져오기
    public BabyData GetCurrentStageData()
    {
        foreach (var data in stageDataList)
        {
            if (data.stage == currentStage)
                return data;
        }
        return null;
    }

    // 현재 씬에 아기가 있는지 체크 (BabyOnGround에서 사용)
    public bool IsBabyInCurrentScene()
    {
        return babyPickedUp &&
               !carryingBaby &&
               babySceneName == SceneManager.GetActiveScene().name;
    }

    // ─── 아기 상태 변경 ──────────────────────────────────

    // 아기 발견 (컷씬 종료 시점에 호출)
    public void PickUpBaby(bool carryImmediately = false)
    {
        babyPickedUp = true;
        carryingBaby = carryImmediately;
        currentStage = BabyStage.Infant;

        babyName = "작은 아기";
        babyAge = 0;

        // BabyStats 초기화
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

    // 아기 들기 (BabyOnGround에서 호출)
    public void CarryBaby()
    {
        carryingBaby = true;
        babySceneName = "";
        SaveState();
    }

    // 아기 내려놓기 / 두고가기 (BabyCarryController에서 호출)
    public void PutDownBaby(Vector3 position, string sceneName)
    {
        carryingBaby = false;
        babyPosition = position;
        babySceneName = sceneName;
        SaveState();
    }

    // 시기 변경 (나중에 BabyGrowthManager가 호출)
    public void GrowToNextStage()
    {
        if (currentStage < BabyStage.Toddler)
        {
            currentStage++;
            Debug.Log($"아기 성장! 현재 시기: {currentStage}");
            SaveState();
        }
    }

    // ─── 저장 / 로드 ─────────────────────────────────────
    public void SaveState()
    {
        PlayerPrefs.SetInt("BabyPickedUp", babyPickedUp ? 1 : 0);
        PlayerPrefs.SetInt("CarryingBaby", carryingBaby ? 1 : 0);
        PlayerPrefs.SetString("BabySceneName", babySceneName);
        PlayerPrefs.SetFloat("BabyPosX", babyPosition.x);
        PlayerPrefs.SetFloat("BabyPosY", babyPosition.y);
        PlayerPrefs.SetFloat("BabyPosZ", babyPosition.z);
        PlayerPrefs.SetString("BabyName", babyName);
        PlayerPrefs.SetInt("BabyAge", babyAge);
        PlayerPrefs.SetInt("BabyStage", (int)currentStage);

        // BabyStats 저장
        PlayerPrefs.SetInt("BabyHealth", babyStats.health);
        PlayerPrefs.SetInt("BabyHunger", babyStats.hunger);
        PlayerPrefs.SetInt("BabyHappiness", babyStats.happiness);
        PlayerPrefs.SetInt("BabyAffection", babyStats.affection);

        PlayerPrefs.Save();
    }

    public void LoadState()
    {
        babyPickedUp = PlayerPrefs.GetInt("BabyPickedUp", 0) == 1;
        carryingBaby = PlayerPrefs.GetInt("CarryingBaby", 0) == 1;
        babySceneName = PlayerPrefs.GetString("BabySceneName", "");
        babyPosition.x = PlayerPrefs.GetFloat("BabyPosX", 0);
        babyPosition.y = PlayerPrefs.GetFloat("BabyPosY", 0);
        babyPosition.z = PlayerPrefs.GetFloat("BabyPosZ", 0);
        babyName = PlayerPrefs.GetString("BabyName", "아기");
        babyAge = PlayerPrefs.GetInt("BabyAge", 0);
        currentStage = (BabyStage)PlayerPrefs.GetInt("BabyStage", 0);

        // BabyStats 로드
        babyStats.health = PlayerPrefs.GetInt("BabyHealth", 100);
        babyStats.hunger = PlayerPrefs.GetInt("BabyHunger", 50);
        babyStats.happiness = PlayerPrefs.GetInt("BabyHappiness", 80);
        babyStats.affection = PlayerPrefs.GetInt("BabyAffection", 50);
    }
}