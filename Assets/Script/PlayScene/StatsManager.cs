using UnityEngine;
using TMPro;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    [Header("UI Reference")]
    public GameObject statsPanel;
    public KeyCode statsKey = KeyCode.I;

    [Header("Player Stats - HOT RELOAD 가능")]
    public float playerHP = 100f;
    public float playerStamina = 100f;

    [Header("UI Text References")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI staminaText;

    private bool isStatsOpen = false;

    void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Stats UI Canvas도 유지
            if (statsPanel != null && statsPanel.transform.root != null)
            {
                DontDestroyOnLoad(statsPanel.transform.root.gameObject);
            }
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
        isStatsOpen = false; // 명확히 초기화
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
}