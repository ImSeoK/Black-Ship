using UnityEngine;

public class CameraManager : MonoBehaviour
{
    void Start()
    {
        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);

        foreach (Camera cam in cameras)
        {
            if (cam.gameObject != gameObject)
            {
                cam.gameObject.SetActive(false);

                // AudioListenerµµ Á¦°Å
                AudioListener listener = cam.GetComponent<AudioListener>();
                if (listener != null)
                {
                    Destroy(listener);
                }
            }
        }
    }
}