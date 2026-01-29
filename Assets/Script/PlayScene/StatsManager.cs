using UnityEngine;
using TMPro;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    [Header("컷씬 상태")]
    public bool forestCutscenePlayed = false;

    [Header("아이 상태")]
    public bool babyPickedUp = false;      // 아이 주웠는지
    public bool carryingBaby = false;      // 현재 들고 있는지
    public string babySceneName = "";      // 아이 놓은 씬
    public Vector3 babyPosition;           // 아이 놓은 위치

    [Header("아이 데이터")]
    public BabyData babyData = new BabyData();

    [Header("UI Reference")]
    public GameObject statsPanel;
    public KeyCode statsKey = KeyCode.Q;

    [Header("Player Stats")]
    public float playerHP = 100f;
    public float playerStamina = 100f;

    [Header("UI Text References")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI staminaText;

    private bool isStatsOpen = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (statsPanel != null && statsPanel.transform.root != null)
            {
                DontDestroyOnLoad(statsPanel.transform.root.gameObject);
            }

            LoadState();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (statsPanel != null)
        {
            statsPanel.SetActive(false);
        }
        isStatsOpen = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(statsKey))
        {
            ToggleStats();
        }

        if (isStatsOpen)
        {
            UpdateStatsDisplay();
        }
    }

    void ToggleStats()
    {
        isStatsOpen = !isStatsOpen;

        if (statsPanel != null)
        {
            statsPanel.SetActive(isStatsOpen);
        }

        if (isStatsOpen)
        {
            UpdateStatsDisplay();
        }
    }

    void UpdateStatsDisplay()
    {
        if (hpText != null)
        {
            hpText.text = $"HP: {playerHP:F0} / 100";
        }

        if (staminaText != null)
        {
            staminaText.text = $"Stamina: {playerStamina:F0} / 100";
        }
    }

    public void SetHP(float value)
    {
        playerHP = Mathf.Clamp(value, 0, 100);
    }

    public void SetStamina(float value)
    {
        playerStamina = Mathf.Clamp(value, 0, 100);
    }

    public void AddHP(float value)
    {
        playerHP = Mathf.Clamp(playerHP + value, 0, 100);
    }

    public void AddStamina(float value)
    {
        playerStamina = Mathf.Clamp(playerStamina + value, 0, 100);
    }

    // ===== 아이 관련 메서드 =====

    public void PickUpBaby(bool carryImmediately = false)
    {
        babyPickedUp = true;
        carryingBaby = carryImmediately; // false면 들고 다니지 않음

        // 아이 초기 데이터
        babyData.babyName = "주운 아기";
        babyData.age = 0;
        babyData.health = 100;
        babyData.hunger = 50;
        babyData.happiness = 80;

        // BaseCamp의 특정 위치에 두기
        if (!carryImmediately)
        {
            babySceneName = "BaseCamp";
            babyPosition = new Vector3(3.5f, -1.5f, 0f); // 요람/침대 위치로 변경
        }

        SaveState();
    }

    public void CarryBaby()
    {
        carryingBaby = true;
        babySceneName = "";
        SaveState();
    }

    public void PutDownBaby(Vector3 position, string sceneName)
    {
        carryingBaby = false;
        babyPosition = position;
        babySceneName = sceneName;
        SaveState();
    }

    // ===== 저장/불러오기 =====

    public void LoadState()
    {
        forestCutscenePlayed = PlayerPrefs.GetInt("ForestCutscenePlayed", 0) == 1;
        babyPickedUp = PlayerPrefs.GetInt("BabyPickedUp", 0) == 1;
        carryingBaby = PlayerPrefs.GetInt("CarryingBaby", 0) == 1;
        babySceneName = PlayerPrefs.GetString("BabySceneName", "");

        babyPosition.x = PlayerPrefs.GetFloat("BabyPosX", 0);
        babyPosition.y = PlayerPrefs.GetFloat("BabyPosY", 0);
        babyPosition.z = PlayerPrefs.GetFloat("BabyPosZ", 0);

        // 아이 데이터
        babyData.babyName = PlayerPrefs.GetString("BabyName", "아기");
        babyData.age = PlayerPrefs.GetInt("BabyAge", 0);
        babyData.health = PlayerPrefs.GetInt("BabyHealth", 100);
        babyData.hunger = PlayerPrefs.GetInt("BabyHunger", 50);
        babyData.happiness = PlayerPrefs.GetInt("BabyHappiness", 80);
    }

    public void SaveState()
    {
        PlayerPrefs.SetInt("ForestCutscenePlayed", forestCutscenePlayed ? 1 : 0);
        PlayerPrefs.SetInt("BabyPickedUp", babyPickedUp ? 1 : 0);
        PlayerPrefs.SetInt("CarryingBaby", carryingBaby ? 1 : 0);
        PlayerPrefs.SetString("BabySceneName", babySceneName);

        PlayerPrefs.SetFloat("BabyPosX", babyPosition.x);
        PlayerPrefs.SetFloat("BabyPosY", babyPosition.y);
        PlayerPrefs.SetFloat("BabyPosZ", babyPosition.z);

        // 아이 데이터
        PlayerPrefs.SetString("BabyName", babyData.babyName);
        PlayerPrefs.SetInt("BabyAge", babyData.age);
        PlayerPrefs.SetInt("BabyHealth", babyData.health);
        PlayerPrefs.SetInt("BabyHunger", babyData.hunger);
        PlayerPrefs.SetInt("BabyHappiness", babyData.happiness);

        PlayerPrefs.Save();
    }
}