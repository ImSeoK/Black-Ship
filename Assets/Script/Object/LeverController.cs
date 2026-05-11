using UnityEngine;

public class LeverController : InteractableObject
{
    [Header("선택 메시지")]
    public string yesButtonText = "가동한다";
    public string noButtonText = "무른다";

    [Header("연결된 문")]
    public DoorController door;

    [Header("애니메이션")]
    public string triggerName = "Activate";

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
            ChoiceUI.Instance.ShowChoice(transform, yesButtonText, noButtonText, OnYes, OnNo);
        }
    }

    void OnYes()
    {
        Debug.Log("=== LeverController OnYes 시작 ===");

        isActivated = true;

        Debug.Log($"Animator: {animator}, Door: {door}");

        // 레버 애니메이션
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
            Debug.Log($"레버 애니메이션 트리거: {triggerName}");
        }
        else
        {
            Debug.LogError("Lever Animator 없음!");
        }

        // 문 열기
        if (door != null)
        {
            Debug.Log($"door.OpenDoor() 호출! door 이름: {door.gameObject.name}");
            door.OpenDoor();
        }
        else
        {
            Debug.LogError("door가 null입니다!");
        }

        Debug.Log("=== OnYes 끝 ===");
    }

    void OnNo()
    {
        Debug.Log("취소");
    }
}