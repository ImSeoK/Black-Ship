using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class LightingController : MonoBehaviour
{
    [Header("Light 자동 검색")]
    [SerializeField] private bool autoFindLight = true;
    [SerializeField] private Light2D targetLight;

    [Header("시간대별 색상")]
    [SerializeField] private Color dawnColor = new Color(1f, 0.7f, 0.5f, 1f);
    [SerializeField] private Color dayColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color eveningColor = new Color(1f, 0.5f, 0.3f, 1f);
    [SerializeField] private Color nightColor = new Color(0.3f, 0.3f, 0.5f, 1f);

    [Header("시간대별 강도")]
    [SerializeField] private float dawnIntensity = 0.8f;
    [SerializeField] private float dayIntensity = 1.0f;
    [SerializeField] private float eveningIntensity = 0.7f;
    [SerializeField] private float nightIntensity = 0.4f;

    [Header("전환 속도")]
    [SerializeField] private float transitionSpeed = 1f;

    private Color targetColor;
    private float targetIntensity;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        FindLight();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeOfDayChanged -= OnTimeOfDayChanged;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindLight();
    }

    void FindLight()
    {
        if (autoFindLight)
        {
            // 현재 씬에서 Light 2D 찾기
            targetLight = FindObjectOfType<Light2D>();

            if (targetLight != null)
            {
                Debug.Log($"Light 자동 검색 성공: {targetLight.gameObject.name}");
            }
        }

        if (targetLight != null && TimeManager.Instance != null)
        {
            // 이벤트 재등록
            TimeManager.Instance.OnTimeOfDayChanged -= OnTimeOfDayChanged;
            TimeManager.Instance.OnTimeOfDayChanged += OnTimeOfDayChanged;

            // 현재 시간대 적용
            OnTimeOfDayChanged(TimeManager.Instance.GetTimeOfDay());
        }
    }

    void Update()
    {
        if (targetLight == null) return;

        targetLight.color = Color.Lerp(targetLight.color, targetColor, Time.deltaTime * transitionSpeed);
        targetLight.intensity = Mathf.Lerp(targetLight.intensity, targetIntensity, Time.deltaTime * transitionSpeed);
    }

    void OnTimeOfDayChanged(TimeManager.TimeOfDay timeOfDay)
    {
        switch (timeOfDay)
        {
            case TimeManager.TimeOfDay.Dawn:
                targetColor = dawnColor;
                targetIntensity = dawnIntensity;
                break;
            case TimeManager.TimeOfDay.Day:
                targetColor = dayColor;
                targetIntensity = dayIntensity;
                break;
            case TimeManager.TimeOfDay.Evening:
                targetColor = eveningColor;
                targetIntensity = eveningIntensity;
                break;
            case TimeManager.TimeOfDay.Night:
                targetColor = nightColor;
                targetIntensity = nightIntensity;
                break;
        }
    }
}