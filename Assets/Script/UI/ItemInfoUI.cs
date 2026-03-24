using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfoUI : MonoBehaviour
{
    [Header("UI ¿ä¼Ò")]
    public Image icon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI weightText;
    public TextMeshProUGUI quantityText;
    public GameObject emptyMessage;

    void Start()
    {
        Clear();
    }

    public void ShowItem(ItemData data)
    {
        if (data == null)
        {
            Clear();
            return;
        }

        emptyMessage.SetActive(false);

        icon.sprite = data.icon;
        icon.enabled = data.icon != null;
        itemNameText.text = data.itemName;
        descriptionText.text = data.description;
        weightText.text = $"WT {data.weight}kg";
        quantityText.text = data.quantity > 1 ? $"x{data.quantity}" : "";
    }

    public void Clear()
    {
        emptyMessage.SetActive(true);
        icon.enabled = false;
        itemNameText.text = "";
        descriptionText.text = "";
        weightText.text = "";
        quantityText.text = "";
    }
}