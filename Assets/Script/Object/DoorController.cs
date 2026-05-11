using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("애니메이션")]
    public string triggerName = "Activate"; // Inspector에서 변경 가능

    private Animator animator;
    private Collider2D doorCollider;
    private bool isOpen = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        doorCollider = GetComponent<Collider2D>();
    }

    public void OpenDoor()
    {
        Debug.Log($"{gameObject.name}.OpenDoor() 호출됨!");
        Debug.Log($"isOpen: {isOpen}, animator: {animator}");

        if (isOpen) return;

        isOpen = true;

        if (animator != null)
        {
            animator.SetTrigger(triggerName);
            Debug.Log($"애니메이션 트리거 실행: {triggerName}");
        }
        else
        {
            Debug.LogError($"{gameObject.name}에 Animator 없음!");
        }

        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }
    }
}