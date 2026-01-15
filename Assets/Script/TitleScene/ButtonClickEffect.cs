using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonClickEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("클릭 효과")]
    public float shakeDuration = 0.1f;
    public float shakeAmount = 5f;
    public float scalePunch = 0.9f;

    [Header("깨지는 이펙트")]
    public Sprite crackSprite;
    public float crackDuration = 0.3f;

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private RectTransform rectTransform;
    private Coroutine effectCoroutine;
    private GameObject currentCrackObj;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.localPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 이전 코루틴 중단
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }

        // 이전 이펙트 즉시 삭제
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
            currentCrackObj = crackObj; // 저장!

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

        // 순간 작아짐
        rectTransform.localScale = originalScale * scalePunch;

        float elapsed = 0f;

        // 흔들림 + 균열 서서히 나타남
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-shakeAmount, shakeAmount);
            float y = Random.Range(-shakeAmount, shakeAmount);

            rectTransform.localPosition = originalPosition + new Vector3(x, y, 0);

            // 균열 페이드 인
            if (crackImage != null)
            {
                Color c = crackImage.color;
                c.a = elapsed / shakeDuration;
                crackImage.color = c;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 원래대로
        rectTransform.localPosition = originalPosition;
        rectTransform.localScale = originalScale;

        // 균열 유지
        if (crackImage != null)
        {
            Color c = crackImage.color;
            c.a = 1f;
            crackImage.color = c;
        }

        // crackDuration 후 페이드 아웃
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

        // 삭제
        if (crackObj != null)
        {
            Destroy(crackObj);
            currentCrackObj = null; // null 처리!
        }

        effectCoroutine = null;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // IPointerUpHandler 인터페이스 구현
        // 필요 없으면 비워둠
    }

}
