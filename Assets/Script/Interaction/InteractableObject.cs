using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("Highlight Settings")]
    public Color outlineColor = new Color(1f, 0.5f, 0f, 1f);
    public float outlineWidth = 0.2f;

    [HideInInspector] public bool interactable = true;

    private SpriteRenderer spriteRenderer;
    private GameObject outlineObject;
    private SpriteRenderer outlineRenderer;
    private bool isHighlighted = false;

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (isHighlighted && outlineRenderer != null && spriteRenderer != null)
            outlineRenderer.sprite = spriteRenderer.sprite;
    }

    public void Highlight()
    {
        if (!interactable || isHighlighted || spriteRenderer == null) return;

        isHighlighted = true;
        outlineObject = new GameObject("Outline");
        outlineObject.transform.SetParent(transform);
        outlineObject.transform.localPosition = Vector3.zero;
        outlineObject.transform.localRotation = Quaternion.identity;
        outlineObject.transform.localScale = Vector3.one * (1f + outlineWidth);

        outlineRenderer = outlineObject.AddComponent<SpriteRenderer>();
        outlineRenderer.sprite = spriteRenderer.sprite;
        outlineRenderer.material = new Material(Shader.Find("Custom/SolidColorSprite"));
        outlineRenderer.color = outlineColor;
        outlineRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        outlineRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
    }

    public void RemoveHighlight()
    {
        if (!isHighlighted) return;
        isHighlighted = false;
        outlineRenderer = null;
        if (outlineObject != null) Destroy(outlineObject);
    }

    public virtual void Interact()
    {
        if (!interactable) return;
        Debug.Log($"{gameObject.name}와 상호작용!");
    }

    void OnDestroy()
    {
        if (outlineObject != null) Destroy(outlineObject);
    }
}