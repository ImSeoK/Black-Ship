using UnityEngine;

public class PlayerHighlight : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Header("하이라이트 설정")]
    public Color highlightColor = new Color(0.5f, 0.8f, 1f, 1f);
    public float outlineWidth = 0.15f;

    private GameObject outlineObject;
    private SpriteRenderer outlineRenderer;
    private bool isHighlighted = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        CheckFadedObjects();
        UpdateOutline();
    }

    void CheckFadedObjects()
    {
        YAxisSorting[] allObjects = FindObjectsByType<YAxisSorting>(FindObjectsSortMode.None);

        bool anyFaded = false;
        foreach (YAxisSorting obj in allObjects)
        {
            if (obj.IsFaded())
            {
                anyFaded = true;
                break;
            }
        }

        // 상태 변화 감지
        if (anyFaded && !isHighlighted)
        {
            Debug.Log("하이라이트 켜기");
            ShowHighlight();
        }
        else if (!anyFaded && isHighlighted)
        {
            Debug.Log("하이라이트 끄기");
            HideHighlight();
        }
    }

    void UpdateOutline()
    {
        if (outlineRenderer != null && spriteRenderer != null)
        {
            // Sorting Order 업데이트
            outlineRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;

            // Sprite 업데이트 (애니메이션 프레임 따라감)
            outlineRenderer.sprite = spriteRenderer.sprite;
        }
    }

    void ShowHighlight()
    {
        if (outlineObject != null) return;

        isHighlighted = true;

        outlineObject = new GameObject("PlayerOutline");
        outlineObject.transform.SetParent(transform);

        // Sprite 높이 계산
        float spriteHeight = spriteRenderer.bounds.size.y;

        // 아래로 오프셋 (발 기준이므로)
        float yOffset = -outlineWidth * spriteHeight * 0.3f; // 0.3 = 조절 가능

        outlineObject.transform.localPosition = new Vector3(0f, yOffset, 0f);
        outlineObject.transform.localRotation = Quaternion.identity;
        outlineObject.transform.localScale = Vector3.one * (1f + outlineWidth);

        outlineRenderer = outlineObject.AddComponent<SpriteRenderer>();
        outlineRenderer.sprite = spriteRenderer.sprite;

        Material solidMat = new Material(Shader.Find("Custom/SolidColorSprite"));
        outlineRenderer.material = solidMat;
        outlineRenderer.color = highlightColor;
        outlineRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
        outlineRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
    }

    void HideHighlight()
    {
        if (outlineObject != null)
        {
            Destroy(outlineObject);
            outlineObject = null;
            outlineRenderer = null;
        }
        isHighlighted = false;
    }

    void OnDestroy()
    {
        if (outlineObject != null)
        {
            Destroy(outlineObject);
        }
    }
}