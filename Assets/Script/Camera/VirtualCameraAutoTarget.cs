using UnityEngine;
using UnityEngine.SceneManagement;

public class VirtualCameraAutoTarget : MonoBehaviour
{
    private Component vcam;

    void Awake()
    {
        var components = GetComponents<Component>();
        foreach (var comp in components)
        {
            if (comp.GetType().Name == "CinemachineCamera")
            {
                vcam = comp;
                break;
            }
        }

        if (vcam == null)
        {
            Debug.LogError($"{gameObject.name}: CinemachineCamera를 찾을 수 없습니다!");
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        FindAndSetPlayer();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindAndSetPlayer();
    }

    void FindAndSetPlayer()
    {
        if (vcam == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning($"{gameObject.name}: Player를 찾을 수 없습니다!");
            return;
        }

        var followProp = vcam.GetType().GetProperty("Follow");

        if (followProp != null)
        {
            followProp.SetValue(vcam, player.transform);
            Debug.Log($"{gameObject.name}: Player 할당 완료!");
        }
        else
        {
            Debug.LogError($"{gameObject.name}: Follow 속성을 찾을 수 없습니다!");
        }
    }
}