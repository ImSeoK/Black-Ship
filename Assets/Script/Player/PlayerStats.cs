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
        PlayerHUD.Instance?.UpdateHUD();
        InventoryUI.Instance?.OnStatsChanged();
    }

    public void LoadState() { }
    public void SaveState() { }
}