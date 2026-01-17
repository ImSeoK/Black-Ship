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
    private DialogueTrigger dialogueTrigger; // 추가!

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        dialogueTrigger = GetComponent<DialogueTrigger>(); // 추가!
    }

    public override void Interact()
    {
        if (isActivated) return;

        if (ChoiceUI.Instance != null)
        {
            ChoiceUI.Instance.ShowChoice(transform, yesButtonText, noButtonText, OnYes, OnNo);
        }
    }

    void OnYes()
    {

        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }

        if (targetCollider != null)
        {
            targetCollider.enabled = false;
        }

        // DialogueTrigger 호출 추가!
        if (dialogueTrigger != null)
        {
            dialogueTrigger.TriggerDialogue();
        }
    }

    void OnNo()
    {
        Debug.Log("취소");
    }
}