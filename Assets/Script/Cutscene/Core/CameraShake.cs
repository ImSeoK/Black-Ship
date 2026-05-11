using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    [Header("Signal Shake ¼³Á¤")]
    public float signalDuration = 0.5f;
    public float signalMagnitude = 0.3f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShakeFromSignal()
    {
        StartCoroutine(ShakeCoroutine(signalDuration, signalMagnitude));
    }

    public void StartContinuousShake(float magnitude)
    {
        StopContinuousShake();
        continuousShakeCoroutine = StartCoroutine(ContinuousShake(magnitude));
    }

    public void StopContinuousShake()
    {
        if (continuousShakeCoroutine != null)
        {
            StopCoroutine(continuousShakeCoroutine);
            continuousShakeCoroutine = null;
        }
    }

    private Coroutine continuousShakeCoroutine;

    IEnumerator ContinuousShake(float magnitude)
    {
        while (true)
        {
            yield return StartCoroutine(ShakeCoroutine(0.15f, magnitude));
        }
    }

    IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0f;
        Vector3 originalPos = Camera.main.transform.localPosition;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            Camera.main.transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Camera.main.transform.localPosition = originalPos;
    }
}