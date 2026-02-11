using UnityEngine;

public class CameraManager : MonoBehaviour
{
    void Start()
    {
        Camera mainCam = GetComponent<Camera>();

        if (mainCam == null)
        {
            Debug.LogError("CameraManager must be on a Camera!");
            return;
        }

        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);

        Debug.Log("Total cameras found: " + cameras.Length);

        foreach (Camera cam in cameras)
        {
            // 자기 자신은 스킵
            if (cam == mainCam)
            {
                Debug.Log("Main Camera (keeping active): " + cam.name);
                continue;
            }

            // 다른 카메라는 비활성화
            Debug.Log("Disabling camera: " + cam.name);
            cam.gameObject.SetActive(false);

            AudioListener listener = cam.GetComponent<AudioListener>();
            if (listener != null)
            {
                Destroy(listener);
            }
        }
    }
}