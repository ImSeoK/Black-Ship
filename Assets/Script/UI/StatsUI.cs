using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class StatBar
{
    public string label;
    public Image fillBar;
    public TextMeshProUGUI valueText;
    public float currentValue;
    public float maxValue;
}

public class StatsUI : MonoBehaviour
{
    [Header("Ω∫≈»πŸ")]
    public StatBar hp;
    public StatBar hunger;
    public StatBar stamina;

    public void Refresh()
    {
        UpdateBar(hp);
        UpdateBar(hunger);
        UpdateBar(stamina);
    }

    void UpdateBar(StatBar stat)
    {
        if (stat.fillBar != null)
            stat.fillBar.fillAmount = stat.currentValue / stat.maxValue;

        if (stat.valueText != null)
            stat.valueText.text = $"{(int)stat.currentValue}/{(int)stat.maxValue}";
    }
}