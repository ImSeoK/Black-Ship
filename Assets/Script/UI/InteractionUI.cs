using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    public static InteractionUI Instance;

    [Header("UI Settings - HOT RELOAD 가능")]
    public GameObject eKeyUI; // E키 UI
    public GameObject fKeyUI; // F키 UI
    public float offsetY = 1f;

    private Transform promptTransform;
    private Transform targetTransform;
    private bool isShowing = false;
    private GameObject currentUI;

    void Awake()
    {
        // 기존 Instance가 파괴되었는지 확인
        if (Instance == null || Instance.gameObject == null)
        {
            Instance = this;
            // 씬 로드 이벤트 등록
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            // 중복이면 기존 Instance에 재연결 신호
            Instance.ReconnectUI();
            Destroy(gameObject);
            return;
        }
    }

    void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // 씬 로드될 때마다 UI 참조 재연결
        ReconnectUI();
    }

    void ReconnectUI()
    {
        if (eKeyUI == null)
        {
            GameObject eKeyObj = GameObject.Find("EKeyIdle");
            if (eKeyObj != null)
            {
                eKeyUI = eKeyObj;
                Debug.Log("EKeyUI 자동 재연결");
            }
        }

        if (fKeyUI == null)
        {
            GameObject fKeyObj = GameObject.Find("FKeyIdle");
            if (fKeyObj != null)
            {
                fKeyUI = fKeyObj;
                Debug.Log("FKeyUI 자동 재연결");
            }
        }
    }

    void Start()
    {
        // UI 참조가 끊겼으면 다시 찾기
        if (eKeyUI == null)
        {
            GameObject eKeyObj = GameObject.Find("EKeyIdle");
            if (eKeyObj != null)
            {
                eKeyUI = eKeyObj;
                Debug.Log("EKeyUI 자동 재연결 완료");
            }
        }

        if (fKeyUI == null)
        {
            GameObject fKeyObj = GameObject.Find("FKeyIdle");
            if (fKeyObj != null)
            {
                fKeyUI = fKeyObj;
                Debug.Log("FKeyUI 자동 재연결 완료");
            }
        }

        if (eKeyUI != null)
        {
            eKeyUI.SetActive(false);
        }
        if (fKeyUI != null)
        {
            fKeyUI.SetActive(false);
        }

        Debug.Log("InteractionUI 초기화 완료");
    }

    void LateUpdate()
    {
        if (isShowing && promptTransform != null && targetTransform != null)
        {
            // Collider 기준으로 상단 위치 계산
            Collider2D collider = targetTransform.GetComponent<Collider2D>();
            float topOffset = 0;

            if (collider != null)
            {
                topOffset = collider.bounds.extents.y;
            }

            Vector3 newPos = targetTransform.position + new Vector3(0, topOffset + offsetY, 0);
            promptTransform.position = newPos;
        }
    }

    public void ShowPrompt(Transform target, bool useFKey = false)
    {
        // 어떤 UI 사용할지 선택
        GameObject uiToShow = useFKey ? fKeyUI : eKeyUI;

        if (uiToShow == null)
        {
            Debug.LogError($"{(useFKey ? "F" : "E")}KeyUI가 연결되지 않음!");
            return;
        }

        // 이전 UI 끄기
        if (currentUI != null && currentUI != uiToShow)
        {
            currentUI.SetActive(false);
        }

        targetTransform = target;
        currentUI = uiToShow;
        promptTransform = currentUI.transform;
        currentUI.SetActive(true);
        isShowing = true;

        Debug.Log($"ShowPrompt 호출됨 - Target: {target.name}, UI: {(useFKey ? "F" : "E")}Key");
    }

    public void HidePrompt()
    {
        if (currentUI != null)
        {
            isShowing = false;
            currentUI.SetActive(false);
            targetTransform = null;
            Debug.Log("HidePrompt 호출됨");
        }
    }
}