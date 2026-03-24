using UnityEngine;

public class BabyCarryController : MonoBehaviour
{
    [Header("�Ʊ� ��������Ʈ")]
    public GameObject babySprite;

    void Update()
    {
        // ���Ʊ��� ���� ��������Ʈ ǥ��
        if (babySprite != null)
        {
            babySprite.SetActive(
                BabyManager.Instance != null &&
                BabyManager.Instance.carryingBaby &&
                BabyManager.Instance.IsInfant()
            );
        }

        if (BabyManager.Instance == null || !BabyManager.Instance.carryingBaby) return;

        if (InputManager.Instance != null && InputManager.Instance.IsPutDownPressed)
        {
            if (BabyManager.Instance.IsInfant())
            {
                // ���Ʊ� - �׳� ��������
                PutDownBaby();
            }
            else
            {
                // ���� �ñ� - �ΰ����� ������
                ShowLeaveChoiceUI();
            }
        }
    }

    void PutDownBaby()
    {
        Vector3 dropPosition = transform.position;
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        BabyManager.Instance.PutDownBaby(dropPosition, currentScene);
        ActivateBabyOnGround();

        Debug.Log($"�Ʊ⸦ {currentScene}�� �������ҽ��ϴ�!");
    }

    void ShowLeaveChoiceUI()
    {
        // ���Ʊ� ���� - ��������/�ΰ����� ����
        if (ChoiceUI.Instance != null)
        {
            ChoiceUI.Instance.ShowChoice(
                transform,
                "�ΰ�����",
                "���",
                OnLeave,
                OnCancel
            );
        }
    }

    void OnLeave()
    {
        // �ΰ����� ���� �� - ���� ���� ��ġ
        Vector3 dropPosition = transform.position;
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        BabyManager.Instance.PutDownBaby(dropPosition, currentScene);
        ActivateBabyOnGround();

        Debug.Log($"�Ʊ⸦ {currentScene}�� �ΰ� ���ϴ�.");
    }

    void OnCancel()
    {
        Debug.Log("����߽��ϴ�.");
    }

    void ActivateBabyOnGround()
    {
        BabyOnGround babyOnGround = FindFirstObjectByType<BabyOnGround>(FindObjectsInactive.Include);

        if (babyOnGround != null)
        {
            babyOnGround.gameObject.SetActive(true);
            babyOnGround.transform.position = BabyManager.Instance.babyPosition;
        }
    }
}