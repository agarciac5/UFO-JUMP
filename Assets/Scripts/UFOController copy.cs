using UnityEngine;
using TMPro;

public class UFOControllerCopy : MonoBehaviour
{
    private GameManager gameManager;
   
    public float rotationSpeed = 50f;

   
    public float jumpForce = 6.5f;
    public float holdForce = 10f;
    public float holdDuration = 0.15f;

  
    public float fallMultiplier = 2.8f;
    public float lowJumpMultiplier = 2.0f;

    
    public TextMeshProUGUI scoreText;
 
    public float score = 0f;

    Rigidbody rb;
    public int lives = 3;
    public int maxLives = 5;
    bool isJumping = false;
    float jumpTime = 0f;
    public float immuneDuration = 1f;
    private bool isImmune = false;
    private float immuneTimer = 0f;
    [HideInInspector] public bool hasShield = false;
    private GameObject activeShield;
    private GameObject activeInmunityShield;
    public Inmunity inmunityData;
  
    void Start()
    {   gameManager = FindFirstObjectByType<GameManager>();
        rb = GetComponent<Rigidbody>();
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString("0");
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (lives <= 0)
        {   
            if (gameManager != null)
            {
                gameManager.GameOver();
              
            }
            Destroy(gameObject);
            return;
        }
       

        if (lives > maxLives)
        {
            lives = maxLives;
        }
        if (isImmune)
        {
            immuneTimer -= Time.deltaTime;
            if (immuneTimer <= 0f)
            {  
                isImmune = false;
            }
        }

        //aumento de score con el tiempo
        score += Time.deltaTime * 10;
        if (scoreText != null)
        {
            scoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
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
        if (!isImmune && activeInmunityShield != null)
        {
            Destroy(activeInmunityShield);
                
            return;
        }
        if (lives > maxLives)
        {
            lives = maxLives;
        }

        if (rb.linearVelocity.y < 0)
        {
            
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
           
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }


    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") && !isImmune)
        {

            if (hasShield && activeShield != null)
            {
                Destroy(activeShield);
                hasShield = false;
                return;
            }
            



            lives = lives - 1;
            isImmune = true;      
            
            immuneTimer = immuneDuration;
             if (inmunityData != null && inmunityData.inmunityVisualPrefab != null)
            {
                activeInmunityShield = Instantiate(inmunityData.inmunityVisualPrefab, transform.position, Quaternion.identity);

                FollowPlayer follow = activeInmunityShield.AddComponent<FollowPlayer>();
                follow.player = transform;
                follow.offset = new Vector3(0f, 0.5f, 0f);
            }
                
        }


    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shield") && !hasShield)
        {
            Shield shieldPickup = other.GetComponent<Shield>();
            if (shieldPickup != null && shieldPickup.shieldVisualPrefab != null)
            {
                activeShield = Instantiate(shieldPickup.shieldVisualPrefab, transform.position, Quaternion.identity);

            
            FollowPlayer follow = activeShield.AddComponent<FollowPlayer>();
            follow.player = transform;
            follow.offset = new Vector3(0f, 0.5f, 0f);
                hasShield = true;
                Destroy(other.gameObject);
            }
        }
    }
   

}
