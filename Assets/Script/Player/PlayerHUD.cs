using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD Instance;

    [Header("HP UI")]
    public Image hpFillImage;

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

    void Start()
    {
        UpdateHUD();
    }

    public void UpdateHUD()
    {
        PlayerStats stats = PlayerStats.Instance;
        if (stats == null) return;

        float ratio = stats.playerHP / stats.maxHP;

        if (hpFillImage != null)
        {
            hpFillImage.fillAmount = ratio;

            if (ratio > 0.6f)
                hpFillImage.color = Color.green;
            else if (ratio > 0.3f)
                hpFillImage.color = Color.yellow;
            else
                hpFillImage.color = Color.red;
        }
    }
}