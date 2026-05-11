using UnityEngine;
using System.Collections;

public class BulletSpawner : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float spawnInterval = 2f;
    public Transform spawnPoint;

    private Coroutine spawnCoroutine;

    public void StartSpawning()
    {
        spawnCoroutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            var obj = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
            obj.GetComponent<Projectile>()?.Fire();
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}