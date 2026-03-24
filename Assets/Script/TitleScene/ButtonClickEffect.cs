using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonClickEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Ŭ�� ȿ��")]
    public float shakeDuration = 0.1f;
    public float shakeAmount = 5f;
    public float scalePunch = 0.9f;
    
    [Header("������ ����Ʈ")]
    public Sprite crackSprite;
    public float crackDuration = 0.3f;
    
    [Header("����")]
    public AudioClip clickSound; // �߰�!
    
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private RectTransform rectTransform;
    private Coroutine effectCoroutine;
    private GameObject currentCrackObj;
    private AudioSource audioSource; // �߰�!

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.localPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (clickSound != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(clickSound);

        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }

        if (currentCrackObj != null)
        {
            Destroy(currentCrackObj);
            currentCrackObj = null;
        }

        Vector3 clickPosition = eventData.position;
        effectCoroutine = StartCoroutine(ClickEffect(clickPosition));
    }

    IEnumerator ClickEffect(Vector3 clickWorldPosition)
    {
        GameObject crackObj = null;
        Image crackImage = null;

        if (crackSprite != null)
        {
            crackObj = new GameObject("Crack");
            currentCrackObj = crackObj; // ����!

            crackObj.transform.SetParent(transform.parent);

            crackImage = crackObj.AddComponent<Image>();
            crackImage.sprite = crackSprite;
            crackImage.raycastTarget = false;
            crackImage.SetNativeSize();

            RectTransform crackRect = crackObj.GetComponent<RectTransform>();
            crackRect.position = clickWorldPosition;

            Color c = crackImage.color;
            c.a = 0;
            crackImage.color = c;
        }

        // ���� �۾���
        rectTransform.localScale = originalScale * scalePunch;

        float elapsed = 0f;

        // ��鸲 + �տ� ������ ��Ÿ��
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-shakeAmount, shakeAmount);
            float y = Random.Range(-shakeAmount, shakeAmount);

            rectTransform.localPosition = originalPosition + new Vector3(x, y, 0);

            // �տ� ���̵� ��
            if (crackImage != null)
            {
                Color c = crackImage.color;
                c.a = elapsed / shakeDuration;
                crackImage.color = c;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // �������
        rectTransform.localPosition = originalPosition;
        rectTransform.localScale = originalScale;

        // �տ� ����
        if (crackImage != null)
        {
            Color c = crackImage.color;
            c.a = 1f;
            crackImage.color = c;
        }

        // crackDuration �� ���̵� �ƿ�
        yield return new WaitForSeconds(crackDuration);

        elapsed = 0f;
        float fadeOutTime = 0.2f;

        while (elapsed < fadeOutTime)
        {
            if (crackImage != null)
            {
                Color c = crackImage.color;
                c.a = 1f - (elapsed / fadeOutTime);
                crackImage.color = c;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // ����
        if (crackObj != null)
        {
            Destroy(crackObj);
            currentCrackObj = null; // null ó��!
        }

        effectCoroutine = null;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // IPointerUpHandler �������̽� ����
        // �ʿ� ������ �����
    }

}
