using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;
    private WeaponManager weaponManager;

    private float lastAttackTime = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        weaponManager = GetComponent<WeaponManager>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        if (weaponManager == null || weaponManager.currentWeaponData == null)
            return;

        // WeaponData에서 쿨타임 가져오기
        float cooldown = weaponManager.currentWeaponData.attackCooldown;

        // 쿨타임 체크
        if (Time.time - lastAttackTime < cooldown)
            return;

        // UI 체크
        if (Time.timeScale == 0)
            return;

        if (DialogueUI.Instance != null && DialogueUI.Instance.IsDialogueActive())
            return;

        if (ChoiceUI.Instance != null && ChoiceUI.Instance.IsActive())
            return;

        // 공격 실행
        PerformAttack();
        lastAttackTime = Time.time;
    }

    void PerformAttack()
    {
        animator.SetTrigger("Attack");

        // WeaponData에서 데미지 가져오기
        int damage = weaponManager.currentWeaponData.damage;
        float range = weaponManager.currentWeaponData.range;

        Debug.Log($"공격! 데미지: {damage}, 사거리: {range}");
    }
}