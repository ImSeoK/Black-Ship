using UnityEngine;

public class InteractableAnimator : InteractableObject
{
    [Header("애니메이션")]
    public string triggerName = "Activate";

    [Header("옵션: 콜라이더 제거")]
    public Collider2D targetCollider;

    private Animator animator;
    private bool isActivated = false;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    public override void Interact()
    {
        if (isActivated) return;

        isActivated = true;

        // 애니메이션 재생
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }

        // 콜라이더 비활성화 (옵션)
        if (targetCollider != null)
        {
            targetCollider.enabled = false;
            Debug.Log($"{targetCollider.gameObject.name} 콜라이더 비활성화!");
        }
    }
}