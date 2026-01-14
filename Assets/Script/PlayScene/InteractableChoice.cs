using UnityEngine;

public class InteractableChoice : InteractableObject
{
    [Header("선택 메시지")]
    public string yesButtonText;
    public string noButtonText;

    [Header("애니메이션")]
    public string triggerName = "Activate";

    [Header("콜라이더 제거")]
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

        if (ChoiceUI.Instance != null)
        {
            // transform 추가, OnNo도 추가!
            ChoiceUI.Instance.ShowChoice(transform, yesButtonText, noButtonText, OnYes, OnNo);
        }
    }

    void OnYes()
    {
        isActivated = true;

        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }

        if (targetCollider != null)
        {
            targetCollider.enabled = false;
        }
    }

    void OnNo()
    {
        Debug.Log("취소");
    }
}