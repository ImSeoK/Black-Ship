using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightController : MonoBehaviour
{
    private Light2D spotLight;

    void Start()
    {
        spotLight = GetComponent<Light2D>();

        if (spotLight == null)
        {
            Debug.LogError("Light2D 컴포넌트가 없습니다!");
        }
    }

    public void SetIntensity(float value)
    {
        Debug.Log($"SetIntensity 호출됨: {value}");

        if (spotLight != null)
        {
            spotLight.enabled = value > 0;
            spotLight.intensity = value;
            Debug.Log($"Intensity 변경: {spotLight.intensity}");
        }
    }
}