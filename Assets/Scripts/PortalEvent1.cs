using UnityEngine;

public class PortalEvent1 : MonoBehaviour
{
    [Header("Portal Settings")]
    public Renderer portalRenderer;      // Renderer del portal
    public float openDuration = 2f;      // Tiempo que tarda en abrir
    public float stayOpenTime = 1f;      // Tiempo que se queda abierto
    public float closeDuration = 2f;     // Tiempo que tarda en cerrar
    public GameObject objectToSpawn;     // Objeto que aparece del portal

    private Material portalMat;

    void Start()
    {
        // Obtener el material y cerrarlo al inicio
        portalMat = portalRenderer.material;
        portalMat.SetFloat("_DissolveAmount", 2f); // cerrado al inicio

        // Iniciar la secuencia automáticamente
        StartCoroutine(PortalSequence());
    }

    private System.Collections.IEnumerator PortalSequence()
    {
        // Abrir portal
        yield return StartCoroutine(ChangeDissolve(1f, 0.341f, openDuration));

        // Esperar mientras el portal está abierto
        yield return new WaitForSeconds(stayOpenTime);

        // Sacar objeto
        if (objectToSpawn)
        {
            Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        }

        // Cerrar portal
        yield return StartCoroutine(ChangeDissolve(0.347f, 2f, closeDuration));
    }

    private System.Collections.IEnumerator ChangeDissolve(float start, float end, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float val = Mathf.Lerp(start, end, t / duration);
            portalMat.SetFloat("_DissolveAmount", val);
            yield return null;
        }
        portalMat.SetFloat("_DissolveAmount", end);
    }
}
