using UnityEngine;

public class PlayerYAxisSorting : MonoBehaviour
{
    [Header("Player Sorting ¼³Á¤")]
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 1000;
    }
}