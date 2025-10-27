using UnityEngine;

public class Shield: MonoBehaviour
{

    public GameObject shieldVisualPrefab;

  
    public float rotationSpeed = 60f;

    void Update()
    {
       
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }
}