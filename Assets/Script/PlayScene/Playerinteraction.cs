using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings - HOT RELOAD 가능")]
    public float interactionRange = 1f;
    public Vector2 rangeOffset = new Vector2(0f, 1.5f); // Y 오프셋 추가
    public KeyCode interactKey = KeyCode.E;
    public LayerMask interactableLayer;

    private InteractableObject currentInteractable;

    void Update()
    {
        // 상호작용 불가 조건
        if (Time.timeScale == 0) return;

        if (DialogueUI.Instance != null && DialogueUI.Instance.IsDialogueActive())
        {
            return;
        }

        if (ChoiceUI.Instance != null && ChoiceUI.Instance.IsActive())
        {
            return;
        }

        // 이제 상호작용 가능
        DetectInteractable();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact();
            }
        }
    }

    void DetectInteractable()
{
    // 오프셋 적용된 위치에서 감지
    Vector2 checkPosition = (Vector2)transform.position + rangeOffset;
    
    RaycastHit2D[] hits = Physics2D.CircleCastAll(checkPosition, interactionRange, Vector2.zero, 0f, interactableLayer);
    
    InteractableObject closestInteractable = null;
    float closestDistance = float.MaxValue;
    Transform closestTransform = null;
    
    foreach (RaycastHit2D hit in hits)
    {
        InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
        if (interactable == null) continue;
        
        float distance = Vector2.Distance(checkPosition, hit.collider.transform.position);

        if (distance < closestDistance)
        {
            closestDistance = distance;
            closestInteractable = interactable;
            closestTransform = hit.collider.transform;
        }
    }

        if (currentInteractable != null && currentInteractable != closestInteractable)
        {
            currentInteractable.RemoveHighlight();
            if (InteractionUI.Instance != null)
            {
                InteractionUI.Instance.HidePrompt();
            }
        }

        if (closestInteractable != null && closestInteractable != currentInteractable)
        {
            closestInteractable.Highlight();
            if (InteractionUI.Instance != null)
            {
                InteractionUI.Instance.ShowPrompt(closestTransform);
            }
        }

        currentInteractable = closestInteractable;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector2 checkPosition = (Vector2)transform.position + rangeOffset;
        Gizmos.DrawWireSphere(checkPosition, interactionRange);
    }
}