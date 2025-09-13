using UnityEngine;

public class UFOController : MonoBehaviour
{

    public float jumpVelocity = 7.0f;
    Rigidbody rb;
    int lives = 1;
    bool jumpRequested = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        if (lives <= 0)
        {
            Destroy(gameObject);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequested = true;
        }


    }

    void FixedUpdate()
    {
        if (lives <= 0)
        {
            return;
        }

        if (jumpRequested)
        {
            Vector3 actualVelocity = rb.linearVelocity;
            actualVelocity.y = jumpVelocity;
            rb.linearVelocity = actualVelocity;

            jumpRequested = false;

        }


    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            lives= lives - 1;
        }
    }
}
