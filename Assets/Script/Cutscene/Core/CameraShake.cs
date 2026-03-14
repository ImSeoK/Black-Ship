using UnityEngine;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        impulseSource = GetComponent<CinemachineImpulseSource>();

        if (impulseSource == null)
            Debug.LogWarning("[CameraShake] CinemachineImpulseSource 없음");
    }

    public void Shake(float duration, float magnitude)
    {
        if (impulseSource == null) return;
        impulseSource.ImpulseDefinition.ImpulseDuration = duration;

        // 랜덤 방향으로 흔들림
        Vector3 randomVelocity = new Vector3(
            Random.Range(-1f, 1f) * magnitude,
            Random.Range(-1f, 1f) * magnitude,
            0f
        );
        impulseSource.GenerateImpulse(randomVelocity);
    }
}