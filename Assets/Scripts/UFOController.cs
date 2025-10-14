using UnityEngine;

public class UFOController : MonoBehaviour
{

    //Parametros para rotacion:
    public float rotationSpeed = 50f;

    //Parametros para el salto:
    public float jumpForce = 6.5f;
    public float holdForce = 10f;
    public float holdDuration = 0.15f;

    //parametros para gravedad
    public float fallMultiplier = 2.8f;
    public float lowJumpMultiplier = 2.0f;
    
    Rigidbody rb;
    int lives = 1;
    bool isJumping = false;
    float jumpTime = 0f;

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
        //rotacion
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        //inicio del salto
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            isJumping = true;
            jumpTime = 0f;

 Vector3 vel = rb.linearVelocity;
    vel.y = 0f;
    rb.linearVelocity = vel;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
        //impulso extra
         if (isJumping && Input.GetKey(KeyCode.Space))
        {
            jumpTime += Time.deltaTime;

            if (jumpTime < holdDuration)
            {
                rb.AddForce(Vector3.up * holdForce * Time.deltaTime, ForceMode.Acceleration);
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }

    }

    void FixedUpdate()
    {
        if (lives <= 0)
        {
            return;
        }

         if (rb.linearVelocity.y < 0)
        {
            // Cayendo → aumenta la gravedad (segunda parábola)
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            // Soltó el botón mientras sube → baja antes (salto corto)
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
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
