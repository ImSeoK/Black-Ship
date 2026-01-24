using UnityEngine;

[System.Serializable]
public class WeaponData
{
    public WeaponType type;
    public string weaponName;
    public string description;
    public Sprite icon;
    public GameObject prefab; // 게임 내 무기 오브젝트
}