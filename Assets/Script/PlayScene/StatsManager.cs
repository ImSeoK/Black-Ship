using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    [Header("컷씬 상태")]
    public bool forestCutscenePlayed = false;

    [Header("아이 상태")]
    public bool babyPickedUp = false;
    public bool carryingBaby = false;
    public string babySceneName = "";
    public Vector3 babyPosition;

    [Header("아이 데이터")]
    public BabyData babyData = new BabyData();

    [Header("UI Reference")]
    public GameObject statsPanel;
    public KeyCode statsKey = KeyCode.Q;

    [Header("Player Stats")]
    public float maxHP = 100f;
    public float playerHP = 100f;
    public float playerStamina = 100f;

    [Header("Combat Settings")]
    public float invincibilityTime = 1f;
    private float lastDamageTime = -999f;

    [Header("HP UI")]
    public Slider hpSlider;
    public Image hpFillImage;

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

        // PlayerUI DontDestroyOnLoad
        GameObject playerUI = GameObject.Find("PlayerUI");
        if (playerUI != null)
        {
            DontDestroyOnLoad(playerUI);
            Debug.Log("PlayerUI set to DontDestroyOnLoad");
        }
        else
        {
            Debug.LogWarning("PlayerUI not found!");
        }

        UpdateHPUI();
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
            hpText.text = $"HP: {playerHP:F0} / {maxHP:F0}";
        }

        if (staminaText != null)
        {
            staminaText.text = $"Stamina: {playerStamina:F0} / 100";
        }
    }

    public void TakeDamage(float damage)
    {
        if (Time.time < lastDamageTime + invincibilityTime)
        {
            Debug.Log("Player is invincible!");
            return;
        }

        playerHP -= damage;
        lastDamageTime = Time.time;

        Debug.Log($"Player took {damage} damage! HP: {playerHP}/{maxHP}");

        if (playerHP <= 0)
        {
            playerHP = 0;
            Die();
        }

        UpdateHPUI();
    }

    void Die()
    {
        Debug.Log("Player died!");
    }

    void UpdateHPUI()
    {
        if (hpSlider != null)
        {
            hpSlider.value = playerHP / maxHP;
        }

        if (hpFillImage != null)
        {
            float hpPercent = playerHP / maxHP;

            if (hpPercent > 0.6f)
                hpFillImage.color = Color.green;
            else if (hpPercent > 0.3f)
                hpFillImage.color = Color.yellow;
            else
                hpFillImage.color = Color.red;
        }
    }

    public void SetHP(float value)
    {
        playerHP = Mathf.Clamp(value, 0, maxHP);
        UpdateHPUI();
    }

    public void SetStamina(float value)
    {
        playerStamina = Mathf.Clamp(value, 0, 100);
    }

    public void AddHP(float value)
    {
        playerHP = Mathf.Clamp(playerHP + value, 0, maxHP);
        UpdateHPUI();
    }

    public void AddStamina(float value)
    {
        playerStamina = Mathf.Clamp(playerStamina + value, 0, 100);
    }

    public void PickUpBaby(bool carryImmediately = false)
    {
        babyPickedUp = true;
        carryingBaby = carryImmediately;

        babyData.babyName = "주워 아기";
        babyData.age = 0;
        babyData.health = 100;
        babyData.hunger = 50;
        babyData.happiness = 80;

        if (!carryImmediately)
        {
            babySceneName = "BaseCamp";
            babyPosition = new Vector3(3.5f, -1.5f, 0f);
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

    public void LoadState()
    {
        forestCutscenePlayed = PlayerPrefs.GetInt("ForestCutscenePlayed", 0) == 1;
        babyPickedUp = PlayerPrefs.GetInt("BabyPickedUp", 0) == 1;
        carryingBaby = PlayerPrefs.GetInt("CarryingBaby", 0) == 1;
        babySceneName = PlayerPrefs.GetString("BabySceneName", "");

        babyPosition.x = PlayerPrefs.GetFloat("BabyPosX", 0);
        babyPosition.y = PlayerPrefs.GetFloat("BabyPosY", 0);
        babyPosition.z = PlayerPrefs.GetFloat("BabyPosZ", 0);

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

        PlayerPrefs.SetString("BabyName", babyData.babyName);
        PlayerPrefs.SetInt("BabyAge", babyData.age);
        PlayerPrefs.SetInt("BabyHealth", babyData.health);
        PlayerPrefs.SetInt("BabyHunger", babyData.hunger);
        PlayerPrefs.SetInt("BabyHappiness", babyData.happiness);

        PlayerPrefs.Save();
    }
}