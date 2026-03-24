using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("패널")]
    public GameObject inventoryPanel;

    [Header("참조")]
    public VestUI vestUI;
    public StatsUI statsUI;

    private bool isOpen = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    void OnEnable()
    {
        PlayerStats.StatsChanged += OnStatsChanged;
    }

    void OnDisable()
    {
        PlayerStats.StatsChanged -= OnStatsChanged;
    }

    void Update()
    {
        if (InputManager.Instance == null) return;

        if (InputManager.Instance.IsInventoryPressed)
            Toggle();

        if (InputManager.Instance.IsPausePressed && isOpen)
            Toggle();
    }

    public void Toggle()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            vestUI?.UpdateOverlay();
            statsUI?.Refresh();
        }
    }

    public void Open()
    {
        isOpen = true;
        inventoryPanel.SetActive(true);
        vestUI?.UpdateOverlay();
        statsUI?.Refresh();
    }

    public void Close()
    {
        isOpen = false;
        inventoryPanel.SetActive(false);
    }

    // PlayerStats.StatsChanged 이벤트 구독 대상
    // PlayerHUD는 별도로 같은 이벤트를 구독하므로 여기서 UpdateHUD 호출 불필요
    void OnStatsChanged()
    {
        SyncStats();

        if (isOpen)
            statsUI?.Refresh();
    }
    public bool IsOpen => isOpen;

    public void SyncStats()
    {
        if (statsUI == null || PlayerStats.Instance == null) return;

        statsUI.hp.currentValue = PlayerStats.Instance.playerHP;
        statsUI.hp.maxValue = PlayerStats.Instance.maxHP;

        statsUI.hunger.currentValue = PlayerStats.Instance.playerHunger;
        statsUI.hunger.maxValue = PlayerStats.Instance.maxHunger;

        statsUI.stamina.currentValue = PlayerStats.Instance.playerStamina;
        statsUI.stamina.maxValue = PlayerStats.Instance.maxStamina;
    }
}