using UnityEngine;

public class RotationUFO : MonoBehaviour
{
    public float inclinationAngle = 15f;
    public float inclinationSpeed = 2f;
    public float rotationSpeed = 50f;


    private float actualInclination = 0f;
    private float lastInclination = 0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float targetInclination = 0f;
        if (transform.position.y > 0.1f)
        {
            targetInclination = -inclinationAngle;
        }
        else if (transform.position.y < -0.1f)
        {
            targetInclination = inclinationAngle;
        }
        actualInclination = Mathf.Lerp(actualInclination, targetInclination, Time.deltaTime * inclinationSpeed);

        transform.Rotate(0f, 0, -lastInclination);
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
        transform.Rotate(0f, 0f, actualInclination);

        lastInclination = actualInclination;
    }
}
