using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

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
                DontDestroyOnLoad(statsPanel.transform.root.gameObject);

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
            statsPanel.SetActive(false);

        isStatsOpen = false;

        GameObject playerUI = GameObject.Find("PlayerUI");
        if (playerUI != null)
            DontDestroyOnLoad(playerUI);
        else
            Debug.LogWarning("PlayerUI not found!");

        UpdateHPUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(statsKey))
            ToggleStats();

        if (isStatsOpen)
            UpdateStatsDisplay();
    }

    void ToggleStats()
    {
        isStatsOpen = !isStatsOpen;
        if (statsPanel != null)
            statsPanel.SetActive(isStatsOpen);

        if (isStatsOpen)
            UpdateStatsDisplay();
    }

    void UpdateStatsDisplay()
    {
        if (hpText != null)
            hpText.text = $"HP: {playerHP:F0} / {maxHP:F0}";

        if (staminaText != null)
            staminaText.text = $"Stamina: {playerStamina:F0} / 100";
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
            hpSlider.value = playerHP / maxHP;

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

    public void LoadState() { }
    public void SaveState() { }
}