using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public Transform player;

    public float spawnInterval = 1.5f; 
    public int obstaclesAtOnce = 3;  
    public float distanceFromPlayer = 15f;
    public float minY = -2f;
    public float maxY = 3f;

    private float timer = 0f;

    void Start()
    {
        if (player == null)
        {
            GameObject ufo = GameObject.FindGameObjectWithTag("Player");
            if (ufo != null) player = ufo.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnObstacles();
        }
    }

    void SpawnObstacles()
    {
        for (int i = 0; i < obstaclesAtOnce; i++)
        {
            float offsetX = distanceFromPlayer + Random.Range(0f, 4f);
            float randomY = Random.Range(minY, maxY);
            Vector3 spawnPos = player.position + new Vector3(offsetX, randomY, 0f);
            Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
        }
    }
}
