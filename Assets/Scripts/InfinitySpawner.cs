using UnityEngine;
using System.Collections;

public class InfinitySpawner : MonoBehaviour
{
    [Header("Objetos que pueden aparecer (obstáculos, portales, etc.)")]
    public GameObject[] spawnPrefabs;

    public float minY = -1f;
    public float maxY = 2f;
    public float spawnX = 10f;

    public float initialSpawnInterval = 2f;
    public float minSpawnInterval = 0.6f;
    public float difficultyIncreaseRate = 0.05f;

    [Header("Aumento de velocidad")]
    public float speedMultiplier = 1f;
    public float speedIncreaseRate = 0.1f;
    public float maxSpeedMultiplier = 3f;

    private float currentSpawnInterval;
    private float timeSinceStart;

    void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnObject();

            // cada 10 segundos reduce el tiempo de espera y aumenta velocidad
            timeSinceStart += currentSpawnInterval;
            if (timeSinceStart > 10f)
            {
                if (currentSpawnInterval > minSpawnInterval)
                    currentSpawnInterval -= difficultyIncreaseRate;

                if (speedMultiplier < maxSpeedMultiplier)
                    speedMultiplier += speedIncreaseRate;

                timeSinceStart = 0f;
            }

            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }

    private void SpawnObject()
    {
        if (spawnPrefabs.Length == 0) return;

        int randomIndex = Random.Range(0, spawnPrefabs.Length);
        GameObject prefab = spawnPrefabs[randomIndex];

        Vector3 spawnPos = new Vector3(spawnX, Random.Range(minY, maxY), 0f);
        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);

        // si el objeto tiene un script con "speed", lo multiplicamos
        var moveScript = obj.GetComponent<ObstacleMove>();
        if (moveScript != null)
        {
            moveScript.speed *= speedMultiplier;
        }
    }
}
