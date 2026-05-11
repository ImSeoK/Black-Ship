using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    [Header("설정")]
    public float speed = 30f;
    public float warningDuration = 1f;
    public Vector2 direction = Vector2.left;

    [Header("오브젝트")]
    public GameObject warningIndicator;
    public GameObject projectileVisual;

    public void Fire()
    {
        StartCoroutine(FireSequence());
    }

    static bool IsDialoguePaused =>
        CutsceneDialogueUI.Instance != null && CutsceneDialogueUI.Instance.IsDirectorPausedByUs;

    IEnumerator FireSequence()
    {
        Vector2 startPosition = transform.position;
        warningIndicator?.SetActive(true);
        projectileVisual?.SetActive(false);

        float elapsed = 0f;
        while (elapsed < warningDuration)
        {
            if (!IsDialoguePaused)
                elapsed += Time.deltaTime;
            yield return null;
        }

        warningIndicator?.SetActive(false);
        projectileVisual?.SetActive(true);
        Debug.Log($"[Projectile] 발사 - eulerAngles: {transform.eulerAngles}, transform.right: {transform.right}, transform.up: {transform.up}");

        while (true)
        {
            if (!IsDialoguePaused)
            {
                transform.position += (Vector3)((Vector2)transform.right * speed * Time.deltaTime);
                if (Vector2.Distance(transform.position, startPosition) > 30f)
                {
                    gameObject.SetActive(false);
                    yield break;
                }
            }
            yield return null;
        }
    }
}