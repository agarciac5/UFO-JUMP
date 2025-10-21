using UnityEngine;
public class SimplePortal : MonoBehaviour
{
    public Renderer portalRenderer;
    public float duration = 2f;        // tiempo para abrir o cerrar
    public GameObject objectToSpawn;   // objeto que sale del portal

    private Material portalMat;

    void Start()
    {
        portalMat = portalRenderer.material;
        portalMat.SetFloat("_Dissolve", 1f); // portal cerrado al inicio
        StartCoroutine(PortalSequence());
    }

    private System.Collections.IEnumerator PortalSequence()
    {
        // Abrir portal
        yield return StartCoroutine(ChangeDissolve(1f, 0f, duration));

        // Sacar objeto
        if (objectToSpawn)
            Instantiate(objectToSpawn, transform.position, Quaternion.identity);

        // Cerrar portal
        yield return StartCoroutine(ChangeDissolve(0f, 1f, duration));
    }

    private System.Collections.IEnumerator ChangeDissolve(float start, float end, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            portalMat.SetFloat("_Dissolve", Mathf.Lerp(start, end, t / duration));
            yield return null;
        }
        portalMat.SetFloat("_Dissolve", end);
    }
}