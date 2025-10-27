using UnityEngine;
using System.Collections;

public class ShieldMove : MonoBehaviour
{
    public float speed = 2f;          // Velocidad de movimiento
    public float lifeTime = 8f;       // Tiempo máximo antes de destruirse
    private bool shrinking = false;   // Evita que se destruya dos veces

    void Start()
    {
        // Se destruye automáticamente después de un tiempo
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Se mueve hacia la izquierda (ajusta según tu juego)
        transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si toca al jugador, activar el escudo y destruirse con efecto
        if (other.CompareTag("Player") && !shrinking)
        {
            StartCoroutine(ShrinkAndDestroy());
        }
    }

    private IEnumerator ShrinkAndDestroy()
    {
        shrinking = true;
        Vector3 startScale = transform.localScale;
        float duration = 0.3f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t / duration);
            yield return null;
        }

        Destroy(gameObject);
    }
}
