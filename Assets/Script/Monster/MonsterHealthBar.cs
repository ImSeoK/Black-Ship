using UnityEngine;
using UnityEngine.UI;

public class MonsterHealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Slider hpSlider;
    public Image fillImage;

    [Header("Settings")]
    public Vector3 offset = new Vector3(0, 1.5f, 0);
    public Color highHpColor = Color.green;
    public Color mediumHpColor = Color.yellow;
    public Color lowHpColor = Color.red;

    private Monster monster;
    private Camera mainCamera;

    void Start()
    {
        monster = GetComponentInParent<Monster>();
        mainCamera = Camera.main;

        if (monster == null)
        {
            Debug.LogError("MonsterHealthBar: Monster component not found in parent!");
        }
    }

    void LateUpdate()
    {
        if (monster == null || mainCamera == null) return;

        // HP 바 업데이트
        float hpPercent = monster.GetHealthPercent();

        if (hpSlider != null)
        {
            hpSlider.value = hpPercent;
        }

        // 색상 변경
        if (fillImage != null)
        {
            if (hpPercent > 0.6f)
                fillImage.color = highHpColor;
            else if (hpPercent > 0.3f)
                fillImage.color = mediumHpColor;
            else
                fillImage.color = lowHpColor;
        }

        // 카메라 향하도록
        transform.rotation = mainCamera.transform.rotation;

        // 위치 조정
        transform.position = monster.transform.position + offset;
    }
}