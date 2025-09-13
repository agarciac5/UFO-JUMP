using UnityEngine;

public class ObstacleMove : MonoBehaviour
{

    public float speed = 2.0f;
    public float extraDistance = 3f;
    public Transform player;
    private bool directionLocked = false;  // ya decidimos la dirección?
    private float dirX = 1f; 
    private float destroyX;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        // Buscar el UFO por tag "Player"
        if (string.IsNullOrEmpty(gameObject.tag) || gameObject.tag == "Untagged")
            gameObject.tag = "Obstacle";

        // Asegurar que el collider NO sea trigger (queremos OnCollisionEnter, no OnTriggerEnter)
        var col = GetComponent<Collider>();
        col.isTrigger = false;

        // Encontrar al UFO por tag "Player"
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go != null) player = go.transform;
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (player == null) return;

        // mover la caja hacia la posición del jugador
        //transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
       if (player == null) return;

        // Fijar dirección y punto de destrucción solo una vez
        if (!directionLocked)
        {
            dirX = Mathf.Sign(player.position.x - transform.position.x);
            if (dirX == 0f) dirX = 1f; // por si estaban alineados
            destroyX = player.position.x + (dirX * extraDistance); // más allá del UFO
            directionLocked = true;
        }

        // Mover continuamente en esa dirección
        transform.Translate(new Vector3(dirX, 0f, 0f) * speed * Time.deltaTime, Space.World);

        // Destruir solo cuando YA lo pasó (un poco más allá)
        if ((dirX > 0 && transform.position.x >= destroyX) ||
            (dirX < 0 && transform.position.x <= destroyX))
        {
            Destroy(gameObject);
        }
    }
}
