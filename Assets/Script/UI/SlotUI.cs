using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotUI : MonoBehaviour
{
    [Header("UI ¿ä¼Ò")]
    public Image icon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI quantityText;
    public Button button;

    private ItemData itemData;
    private ItemInfoUI itemInfoUI;

    public void Setup(ItemData data, ItemInfoUI infoUI)
    {
        itemData = data;
        itemInfoUI = infoUI;

        if (data != null)
        {
            icon.sprite = data.icon;
            icon.enabled = data.icon != null;
            itemNameText.text = data.itemName;
            quantityText.text = data.quantity > 1 ? $"x{data.quantity}" : "";
            button.interactable = true;
        }
        else
        {
            icon.enabled = false;
            itemNameText.text = "";
            quantityText.text = "";
            button.interactable = false;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClicked);
    }

    void OnClicked()
    {
        if (itemData == null) return;
        itemInfoUI?.ShowItem(itemData);
    }
}