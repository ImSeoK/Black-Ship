using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;

    [Header("공격 설정")]
    public float attackCooldown = 0.5f;
    private float lastAttackTime = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 좌클릭 공격
        if (Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        // 쿨타임 체크
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        // UI 열려있으면 공격 안 함
        if (Time.timeScale == 0)
            return;

        // 대화 중이면 공격 안 함
        if (DialogueUI.Instance != null && DialogueUI.Instance.IsDialogueActive())
            return;

        // 선택지 중이면 공격 안 함
        if (ChoiceUI.Instance != null && ChoiceUI.Instance.IsActive())
            return;

        // 공격 실행
        animator.SetTrigger("Attack");
        lastAttackTime = Time.time;
    }
}