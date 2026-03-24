using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class BelongingUI : MonoBehaviour
{
    [Header("참조")]
    public Transform slotsParent;
    public GameObject slotPrefab;
    public ItemInfoUI itemInfoUI;

    [Header("UI")]
    public TextMeshProUGUI pouchNameLabel;

    private List<SlotUI> slotPool = new List<SlotUI>();
    private string currentPouchID;

    public void ShowPouch(string pouchID)
    {
        currentPouchID = pouchID;
        itemInfoUI?.Clear();

        PouchData pouch = InventoryData.Instance?.GetPouch(pouchID);
        if (pouch == null) return;

        // 라벨 업데이트 추가
        if (pouchNameLabel != null)
            pouchNameLabel.text = pouch.pouchName;

        // 슬롯 풀 확보
        while (slotPool.Count < pouch.maxSlots)
        {
            GameObject obj = Instantiate(slotPrefab, slotsParent);
            slotPool.Add(obj.GetComponent<SlotUI>());
        }

        // 슬롯 채우기
        for (int i = 0; i < slotPool.Count; i++)
        {
            slotPool[i].gameObject.SetActive(i < pouch.maxSlots);
            if (i < pouch.maxSlots)
            {
                ItemData item = i < pouch.items.Count ? pouch.items[i] : null;
                slotPool[i].Setup(item, itemInfoUI);
            }
        }
    }

    public void Refresh()
    {
        if (!string.IsNullOrEmpty(currentPouchID))
            ShowPouch(currentPouchID);
    }
}