using UnityEngine;
using System.Collections;

public class SpaceTeleport : MonoBehaviour
{
    [Header("텔레포트 설정")]
    public Transform destination;
    public SpaceTeleport destinationTeleport;

    private bool isCooling = false;

    public void SetCooldown()
    {
        StartCoroutine(Cooldown());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (isCooling) return;

        other.transform.position = destination.position;
        StartCoroutine(Cooldown());
        destinationTeleport?.SetCooldown();
    }

    IEnumerator Cooldown()
    {
        isCooling = true;
        yield return new WaitForSeconds(1f);
        isCooling = false;
    }
}