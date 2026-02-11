using UnityEngine;

public class YAxisSorting : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    void LateUpdate()
    {
        if (spriteRenderer == null) return;

        // Y 좌표를 sortingOrder로 변환
        // Y가 높을수록 뒤에 (작은 값)
        spriteRenderer.sortingOrder = (int)(5000 - transform.position.y * 10);
    }
}