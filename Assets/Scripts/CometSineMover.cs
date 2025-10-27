using UnityEngine;

public class CometSineMover : MonoBehaviour
{
    // ---- Oscilación vertical ----
    public float verticalAmplitude = 1.2f;   
    public float verticalFrequency = 1.3f;   
    public float phaseOffset = 0f;           

    // ---- Colisión como los meteoritos ----
    public bool autoEnsureCollider = true;   
    public bool colliderIsTrigger = false;   
    public bool autoSetObstacleTag = true;   

    Transform tr;
    float t0;
    float lastOffset;

    void Awake()
    {
        tr = transform;

        
        if (autoSetObstacleTag && gameObject.tag != "Obstacle")
            gameObject.tag = "Obstacle";

        
        if (autoEnsureCollider)
        {
            Collider col = GetComponent<Collider>();
            if (col == null)
            {
                
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
        
        if (lastOffset != 0f)
            tr.position -= Vector3.up * lastOffset;

        float elapsed = Time.time - t0;
        float offset = Mathf.Sin((elapsed * verticalFrequency * 2f * Mathf.PI) + phaseOffset) * verticalAmplitude;

        
        tr.position += Vector3.up * offset;

        lastOffset = offset;
    }

    void OnDisable()
    {
        
        if (lastOffset != 0f && tr != null)
            tr.position -= Vector3.up * lastOffset;

        lastOffset = 0f;
    }
}
