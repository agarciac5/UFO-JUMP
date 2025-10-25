using UnityEngine;

public class Inmunity: MonoBehaviour
{

    public GameObject inmunityVisualPrefab;

  
    public float rotationSpeed = 60f;

    void Update()
    {
       
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }
}