using UnityEngine;

public class CometSineMover : MonoBehaviour
{
    // ---- Oscilación vertical ----
    public float verticalAmplitude = 1.2f;   // altura de la oscilación
    public float verticalFrequency = 1.3f;   // oscilaciones por segundo
    public float phaseOffset = 0f;           // desfase (radianes)

    // ---- Colisión como los meteoritos ----
    public bool autoEnsureCollider = true;   // si no hay collider, agrega uno
    public bool colliderIsTrigger = false;   // deja false para colisión física (OnCollisionEnter en UFO)
    public bool autoSetObstacleTag = true;   // pone tag "Obstacle" en el root si no lo tiene

    Transform tr;
    float t0;
    float lastOffset;

    void Awake()
    {
        tr = transform;

        // Tag igual que los meteoritos
        if (autoSetObstacleTag && gameObject.tag != "Obstacle")
            gameObject.tag = "Obstacle";

        // Asegurar collider para que el UFO pueda colisionar
        if (autoEnsureCollider)
        {
            Collider col = GetComponent<Collider>();
            if (col == null)
            {
                // Si tu mesh es irregular, cambia BoxCollider por MeshCollider (convexo)
                col = gameObject.AddComponent<BoxCollider>();
            }
            col.isTrigger = colliderIsTrigger;
        }
    }

    void OnEnable()
    {
        t0 = Time.time;
        lastOffset = 0f;
    }

    void LateUpdate()
    {
        // Quitamos offset anterior para evitar deriva con el movimiento hacia adelante
        if (lastOffset != 0f)
            tr.position -= Vector3.up * lastOffset;

        float elapsed = Time.time - t0;
        float offset = Mathf.Sin((elapsed * verticalFrequency * 2f * Mathf.PI) + phaseOffset) * verticalAmplitude;

        // Aplicamos el nuevo offset vertical (eje mundo, como tus meteoritos)
        tr.position += Vector3.up * offset;

        lastOffset = offset;
    }

    void OnDisable()
    {
        // Limpiar el offset aplicado si se desactiva
        if (lastOffset != 0f && tr != null)
            tr.position -= Vector3.up * lastOffset;

        lastOffset = 0f;
    }
}
