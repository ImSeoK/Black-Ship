using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("HP")]
    public float maxHP = 100f;
    public float playerHP = 100f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float playerStamina = 100f;

    [Header("Hunger")]
    public float maxHunger = 100f;
    public float playerHunger = 100f;

    static class SaveKeys
    {
        public const string HP      = "PlayerHP";
        public const string Stamina = "PlayerStamina";
        public const string Hunger  = "PlayerHunger";
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

    // HP
    public void SetHP(float value)
    {
        playerHP = Mathf.Clamp(value, 0, maxHP);
        OnStatsChanged();
    }

    public void AddHP(float value)
    {
        playerHP = Mathf.Clamp(playerHP + value, 0, maxHP);
        OnStatsChanged();
    }

    // Stamina
    public void SetStamina(float value)
    {
        playerStamina = Mathf.Clamp(value, 0, maxStamina);
        OnStatsChanged();
    }

    public void AddStamina(float value)
    {
        playerStamina = Mathf.Clamp(playerStamina + value, 0, maxStamina);
        OnStatsChanged();
    }

    // Hunger
    public void SetHunger(float value)
    {
        playerHunger = Mathf.Clamp(value, 0, maxHunger);
        OnStatsChanged();
    }

    public void AddHunger(float value)
    {
        playerHunger = Mathf.Clamp(playerHunger + value, 0, maxHunger);
        OnStatsChanged();
    }

    void OnStatsChanged()
    {
        StatsChanged?.Invoke();
        SaveState();
    }

    // PlayerHUD, InventoryUI 등이 직접 참조 대신 이 이벤트를 구독
    public static event System.Action StatsChanged;

    public void LoadState()
    {
        playerHP      = PlayerPrefs.GetFloat(SaveKeys.HP,      maxHP);
        playerStamina = PlayerPrefs.GetFloat(SaveKeys.Stamina, maxStamina);
        playerHunger  = PlayerPrefs.GetFloat(SaveKeys.Hunger,  maxHunger);
    }

    public void SaveState()
    {
        PlayerPrefs.SetFloat(SaveKeys.HP,      playerHP);
        PlayerPrefs.SetFloat(SaveKeys.Stamina, playerStamina);
        PlayerPrefs.SetFloat(SaveKeys.Hunger,  playerHunger);
        PlayerPrefs.Save();
    }
}
