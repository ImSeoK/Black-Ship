using UnityEngine;

public class ButtonEffectManager : MonoBehaviour
{
    public static ButtonEffectManager Instance;

    [System.Serializable]
    public class WeaponEffect
    {
        public Sprite effectSprite;
        public AudioClip soundClip;
    }

    [Header("무기별 이펙트")]
    public WeaponEffect fistEffect;
    public WeaponEffect swordEffect;
    public WeaponEffect gunEffect;

    private WeaponEffect currentEffect;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 기본값
        currentEffect = fistEffect;
    }

    public void SetWeaponEffect(string weaponType)
    {
        switch (weaponType)
        {
            case "Fist":
                currentEffect = fistEffect;
                break;
            case "Sword":
                currentEffect = swordEffect;
                break;
            case "Gun":
                currentEffect = gunEffect;
                break;
        }
    }

    public Sprite GetCurrentSprite()
    {
        return currentEffect?.effectSprite;
    }

    public AudioClip GetCurrentSound()
    {
        return currentEffect?.soundClip;
    }
}