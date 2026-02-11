using UnityEngine;
using System.Collections;

public class CinemachineShaker : MonoBehaviour
{
    public static CinemachineShaker Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) yield break;

        // Cinemachine Brain 끄기
        var brain = mainCamera.GetComponent<MonoBehaviour>();
        bool brainWasEnabled = false;

        foreach (var comp in mainCamera.GetComponents<MonoBehaviour>())
        {
            if (comp.GetType().Name == "CinemachineBrain")
            {
                brainWasEnabled = comp.enabled;
                comp.enabled = false;
                break;
            }
        }

        // 기존 CameraShake와 동일
        Vector3 originalPosition = mainCamera.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            mainCamera.transform.position = originalPosition + new Vector3(x, y, 0f);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        mainCamera.transform.position = originalPosition;

        // Cinemachine Brain 다시 켜기
        foreach (var comp in mainCamera.GetComponents<MonoBehaviour>())
        {
            if (comp.GetType().Name == "CinemachineBrain")
            {
                comp.enabled = brainWasEnabled;
                break;
            }
        }
    }
}