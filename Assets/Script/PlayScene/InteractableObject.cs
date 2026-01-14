using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("Highlight Settings - HOT RELOAD 가능")]
    public Color outlineColor = new Color(1f, 0.5f, 0f, 1f);
    public float outlineWidth = 0.2f;

    private SpriteRenderer spriteRenderer; // 기존 (레버 본체)
    private GameObject outlineObject; // 기존
    private bool isHighlighted = false; // 기존
    private SpriteRenderer outlineRenderer; // 추가 (외곽선)

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        // 하이라이트 중이면 스프라이트 동기화
        if (isHighlighted && outlineRenderer != null && spriteRenderer != null)
        {
            outlineRenderer.sprite = spriteRenderer.sprite;
        }
    }

    public void Highlight()
    {
        if (isHighlighted || spriteRenderer == null) return;

        isHighlighted = true;

        outlineObject = new GameObject("Outline");
        outlineObject.transform.SetParent(transform);
        outlineObject.transform.localPosition = Vector3.zero;
        outlineObject.transform.localRotation = Quaternion.identity;
        outlineObject.transform.localScale = Vector3.one * (1f + outlineWidth);

        outlineRenderer = outlineObject.AddComponent<SpriteRenderer>(); // 저장
        outlineRenderer.sprite = spriteRenderer.sprite;

        Material solidMat = new Material(Shader.Find("Custom/SolidColorSprite"));
        outlineRenderer.material = solidMat;
        outlineRenderer.color = outlineColor;
        outlineRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        outlineRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
    }

    public void RemoveHighlight()
    {
        if (!isHighlighted) return;

        isHighlighted = false;
        outlineRenderer = null; // 추가

        if (outlineObject != null)
        {
            Destroy(outlineObject);
        }
    }

    public virtual void Interact()
    {
        Debug.Log($"{gameObject.name}와 상호작용!");
    }

    void OnDestroy()
    {
        if (outlineObject != null)
        {
            Destroy(outlineObject);
        }
    }
}