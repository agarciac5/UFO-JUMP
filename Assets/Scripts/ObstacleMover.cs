using UnityEngine;
using System.Collections;


public class ObstacleMove : MonoBehaviour
{

    public float speed = 2.0f;
    public float extraDistance = 5f;
    public Transform player;
    private bool directionLocked = false; 
    private float dirX = 1f;
    private float destroyX;
    private bool shrinking = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
       
        if (string.IsNullOrEmpty(gameObject.tag) || gameObject.tag == "Untagged")
            gameObject.tag = "Obstacle";

        
        var col = GetComponent<Collider>();
        col.isTrigger = false;

        
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go != null) player = go.transform;

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        if (player == null || shrinking) return;

        
        if (!directionLocked)
        {
            dirX = Mathf.Sign(player.position.x - transform.position.x);
            if (dirX == 0f) dirX = 1f; 
            destroyX = player.position.x + (dirX * extraDistance);
            directionLocked = true;
        }

      
        transform.Translate(new Vector3(dirX, 0f, 0f) * speed * Time.deltaTime, Space.World);

       
        if ((dirX > 0 && transform.position.x >= destroyX) ||
            (dirX < 0 && transform.position.x <= destroyX))
        {
            Destroy(gameObject);
        }
    } 

private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(ShrinkAndDestroy());
        }
    }

    private IEnumerator ShrinkAndDestroy()
    {
        shrinking = true;
        Vector3 originalScale = transform.localScale;
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            yield return null;
        }

        Destroy(gameObject);
    }



}
