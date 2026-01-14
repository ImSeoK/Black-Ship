using UnityEngine;

public class YAxisSorting : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Header("Y축 정렬")]
    public int sortingOrderBase = 5000;
    public int offset = 0;
    public bool isStatic = false;

    [Header("반투명 설정")]
    public float fadeAlpha = 0.5f;
    public float checkRadius = 1.5f;

    private float lastY;
    private Color originalColor;
    private bool isFaded = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        UpdateSortingOrder();
    }

    void LateUpdate()
    {
        // Y축 정렬
        if (!isStatic)
        {
            if (Mathf.Abs(transform.position.y - lastY) > 0.01f)
            {
                UpdateSortingOrder();
            }
        }

        // 반투명 체크
        CheckPlayerBehind();
    }

    void UpdateSortingOrder()
    {
        if (spriteRenderer != null)
        {
            lastY = transform.position.y;
            int order = (int)(sortingOrderBase - lastY * 100) + offset;
            spriteRenderer.sortingOrder = order;
        }
    }

    void CheckPlayerBehind()
    {
        // Player 본인은 체크 안 함
        if (gameObject.CompareTag("Player")) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Bounds spriteBounds = spriteRenderer.bounds;

        bool inXRange = Mathf.Abs(player.transform.position.x - transform.position.x) < spriteBounds.extents.x;
        bool inYRange = player.transform.position.y > spriteBounds.min.y &&
                        player.transform.position.y < spriteBounds.max.y;

        bool playerBehind = inXRange && inYRange;

        if (playerBehind && !isFaded)
        {
            Color fadedColor = originalColor;
            fadedColor.a = fadeAlpha;
            spriteRenderer.color = fadedColor;
            isFaded = true;
            Debug.Log($"[{gameObject.name}] 반투명 ON");
        }
        else if (!playerBehind && isFaded)
        {
            spriteRenderer.color = originalColor;
            isFaded = false;
            Debug.Log($"[{gameObject.name}] 반투명 OFF");
        }
    }

    public bool IsFaded()
    {
        return isFaded;
    }
}