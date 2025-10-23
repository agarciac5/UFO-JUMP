using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Configuración del spawn")]
    public GameObject obstaclePrefab;     // Prefab del obstáculo
    public Transform player;               // Referencia al jugador
    public float spawnDistance = 15f;      // Qué tan lejos del jugador aparecen
    public float minY = -2f;               // Altura mínima de aparición
    public float maxY = 3f;                // Altura máxima de aparición
    public float spawnInterval = 2f;       // Cada cuánto tiempo aparece uno
    public float difficultyIncreaseRate = 0.05f; // Cuánto aumenta la velocidad por segundo

    private float timer = 0f;
    private float timeSinceStart = 0f;

    void Start()
    {
        if (player == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) player = go.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        timer += Time.deltaTime;
        timeSinceStart += Time.deltaTime;

        // Generar obstáculo cada cierto tiempo
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnObstacle();
        }
    }

    void SpawnObstacle()
    {
        if (obstaclePrefab == null || player == null) return;

        // Posición de aparición (a la derecha o izquierda del jugador)
        Vector3 spawnPos = player.position + new Vector3(spawnDistance, Random.Range(minY, maxY), 0f);
        GameObject newObstacle = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);

        // Aumentar velocidad con el tiempo (modo infinito progresivo)
        ObstacleMove obstacleScript = newObstacle.GetComponent<ObstacleMove>();
        if (obstacleScript != null)
        {
            obstacleScript.speed += timeSinceStart * difficultyIncreaseRate;
        }
    }
}
