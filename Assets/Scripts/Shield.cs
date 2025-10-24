using UnityEngine;

public class Shield: MonoBehaviour
{

    public GameObject shieldVisualPrefab;

  
    public float rotationSpeed = 60f;

    void Update()
    {
        // Gira el pickup para que se vea más llamativo
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }
}