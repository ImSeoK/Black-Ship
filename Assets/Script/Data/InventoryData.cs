using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ItemData
{
    public string itemName;
    public string description;
    public float weight;
    public int quantity;
    public Sprite icon;
}

[System.Serializable]
public class PouchData
{
    public string pouchID;
    public string pouchName;
    public int maxSlots;
    public List<ItemData> items = new List<ItemData>();
}

public class InventoryData : MonoBehaviour
{
    public static InventoryData Instance;

    [Header("적재량")]
    public float maxWeight = 30f;
    public float currentWeight = 0f;

    [Header("파우치")]
    public List<PouchData> pouches = new List<PouchData>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public float GetWeightRatio()
    {
        return currentWeight / maxWeight;
    }

    public PouchData GetPouch(string pouchID)
    {
        return pouches.Find(p => p.pouchID == pouchID);
    }

    public bool AddItem(string pouchID, ItemData item)
    {
        PouchData pouch = GetPouch(pouchID);
        if (pouch == null) return false;
        if (pouch.items.Count >= pouch.maxSlots) return false;

        pouch.items.Add(item);
        currentWeight += item.weight;
        return true;
    }

    public void RemoveItem(string pouchID, ItemData item)
    {
        PouchData pouch = GetPouch(pouchID);
        if (pouch == null) return;

        pouch.items.Remove(item);
        currentWeight -= item.weight;
        if (currentWeight < 0) currentWeight = 0;
    }
}