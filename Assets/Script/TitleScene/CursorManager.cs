using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [Header("커서 이미지")]
    public Texture2D cursorDefault;

    [Header("핫스팟 (클릭 위치)")]
    public Vector2 hotspot = Vector2.zero;

    void Start()
    {
        SetCursor(cursorDefault);
    }

    public void SetCursor(Texture2D cursor)
    {
        if (cursor != null)
        {
            Cursor.SetCursor(cursor, hotspot, CursorMode.Auto);
        }
    }
}