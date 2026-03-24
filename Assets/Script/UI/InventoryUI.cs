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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            Toggle();

        if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
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

    public void OnStatsChanged()
    {
        PlayerHUD.Instance?.UpdateHUD();

        // 열려있든 닫혀있든 항상 내부 데이터 동기화
        SyncStats();

        // 화면 갱신은 열려있을 때만
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