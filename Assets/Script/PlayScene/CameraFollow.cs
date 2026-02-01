using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    [Header("Debug (Read Only)")]
    [SerializeField] private Vector3 tempOffset = Vector3.zero;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("CameraFollow: Scene loaded - " + scene.name);
        FindPlayer();
    }

    void Start()
    {
        Debug.Log("CameraFollow Start!");
        FindPlayer();
    }

    void LateUpdate()
    {
        if (target == null)
        {
            FindPlayer();
            if (target == null) return;
        }

        Vector3 desiredPosition = target.position + offset + tempOffset;

        // 로그 추가 (tempOffset이 있을 때만)
        if (tempOffset.magnitude > 0.01f)
        {
            Debug.Log("LateUpdate - target: " + target.position + ", tempOffset: " + tempOffset + ", desired: " + desiredPosition);
        }

        transform.position = desiredPosition;
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            Debug.Log("CameraFollow: Player found at " + player.transform.position);
        }
        else
        {
            Debug.LogWarning("CameraFollow: Player not found!");
        }
    }

    public void SetTempOffset(Vector3 newOffset)
    {
        tempOffset = newOffset;
    }

    public void AddTempOffset(Vector3 additionalOffset)
    {
        tempOffset += additionalOffset;
    }

    public void ResetTempOffset()
    {
        tempOffset = Vector3.zero;
    }
}