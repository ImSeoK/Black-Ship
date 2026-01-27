using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [Header("커서 이미지")]
    public Texture2D defaultCursor; // 주먹
    public Texture2D spearCursor;   // 창
    public Texture2D swordCursor;   // 검
    public Texture2D bowCursor;     // 활

    [Header("커서 핫스팟")]
    public Vector2 hotspot = new Vector2(16, 16); // 커서 중심점

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
    }

    void Start()
    {
        SetDefaultCursor();
    }

    public void SetDefaultCursor()
    {
        if (defaultCursor != null)
        {
            Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    public void SetWeaponCursor(WeaponType weaponType)
    {
        Texture2D cursor = null;

        switch (weaponType)
        {
            case WeaponType.Spear:
                cursor = spearCursor;
                break;
            case WeaponType.Sword:
                cursor = swordCursor;
                break;
            case WeaponType.Bow:
                cursor = bowCursor;
                break;
            default:
                SetDefaultCursor();
                return;
        }

        if (cursor != null)
        {
            Cursor.SetCursor(cursor, hotspot, CursorMode.Auto);
        }
    }
}