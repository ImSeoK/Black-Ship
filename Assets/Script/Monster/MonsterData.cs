using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Data", menuName = "Game/Monster Data")]
public class MonsterData : ScriptableObject
{
    [Header("기본 정보")]
    public string monsterName = "몬스터";
    public Sprite sprite;
    public RuntimeAnimatorController animatorController;

    [Header("기본 스탯")]
    public float maxHealth = 100f;
    public float moveSpeed = 2f;
    public float chaseSpeed = 4f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;

    [Header("AI 설정")]
    public float detectionRange = 5f;
    public float chaseGiveUpRange = 10f;
    public float attackRange = 1.5f;
    public float wanderWaitTime = 2f;
    public float wanderDistance = 3f;

    [Header("보상")]
    public int expReward = 10;
}