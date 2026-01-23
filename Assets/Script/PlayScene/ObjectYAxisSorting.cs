using UnityEngine;

public class ObjectYAxisSorting : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private GameObject player;

    void Start()
    {
        // SpriteRenderer 찾기 (자식 포함)
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogWarning($"{gameObject.name}에 SpriteRenderer가 없습니다!");
            enabled = false; // 스크립트 비활성화
            return;
        }

        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("Player를 찾을 수 없습니다!");
            enabled = false;
        }
    }

    void LateUpdate()
    {
        if (spriteRenderer == null || player == null) return;

        // 오브젝트 Y < Player Y → 앞
        if (transform.position.y < player.transform.position.y)
        {
            spriteRenderer.sortingOrder = 1001;
        }
        else
        {
            spriteRenderer.sortingOrder = 999;
        }
    }
}