using UnityEngine;
using UnityEngine.UI;
using System;

public class ChoiceUI : MonoBehaviour
{
    public static ChoiceUI Instance;

    [Header("UI")]
    public GameObject choicePanel;
    public Button yesButton;
    public Button noButton;

    [Header("위치 설정")]
    public Vector3 offset = new Vector3(2f, 0.5f, 0f);

    private Action onYes;
    private Action onNo;
    private int selectedIndex = 0; // 0 = Yes, 1 = No

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

    void Start()
    {
        choicePanel.SetActive(false);

        yesButton.onClick.AddListener(OnYesClick);
        noButton.onClick.AddListener(OnNoClick);
    }

    void Update()
    {
        if (!choicePanel.activeSelf) return;

        // ESC로 취소
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnNoClick();
            return;
        }

        // 좌우 화살표 입력 (경계 체크)
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (selectedIndex > 0) // Yes(0)에서 왼쪽 막기
            {
                selectedIndex--;
                UpdateButtonHighlight();
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (selectedIndex < 1) // No(1)에서 오른쪽 막기
            {
                selectedIndex++;
                UpdateButtonHighlight();
            }
        }

        // Enter/Space로 선택
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (selectedIndex == 0)
                OnYesClick();
            else
                OnNoClick();
        }
    }

    public void ShowChoice(Transform target, string yesText, string noText, Action onYesAction, Action onNoAction)
    {
        Debug.Log("ShowChoice 호출됨!");

        yesButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = yesText;
        noButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = noText;

        onYes = onYesAction;
        onNo = onNoAction;

        selectedIndex = 0;
        UpdateButtonHighlight();

        Debug.Log($"choicePanel 활성화 전: {choicePanel.activeSelf}");
        choicePanel.SetActive(true);
        Debug.Log($"choicePanel 활성화 후: {choicePanel.activeSelf}");

        Time.timeScale = 0f;
    }

    void UpdateButtonHighlight()
    {
        // 선택된 버튼 강조
        ColorBlock yesColors = yesButton.colors;
        ColorBlock noColors = noButton.colors;

        if (selectedIndex == 0)
        {
            yesColors.normalColor = Color.yellow;
            noColors.normalColor = Color.white;
        }
        else
        {
            yesColors.normalColor = Color.white;
            noColors.normalColor = Color.yellow;
        }

        yesButton.colors = yesColors;
        noButton.colors = noColors;
    }

    public bool IsActive()
    {
        return choicePanel.activeSelf;
    }

    void OnYesClick()
    {
        choicePanel.SetActive(false);
        Time.timeScale = 1.0f;
        onYes?.Invoke();
    }

    void OnNoClick()
    {
        choicePanel.SetActive(false);
        Time.timeScale = 1.0f; 
        onNo?.Invoke();
    }
}