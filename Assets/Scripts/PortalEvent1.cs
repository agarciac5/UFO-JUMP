using UnityEngine;

public class PortalEvent1 : MonoBehaviour
{
   
    public Renderer portalRenderer;     //acceder material del portal
    public float openDuration = 2f;      
    public float stayOpenTime = 1f;     
    public float closeDuration = 2f;
    
    public float delayBeforeStart = 1.5f;    
    public GameObject objectToSpawn;     

    private Material portalMat;

    void Start()
    {
        portalMat = portalRenderer.material;
        portalMat.SetFloat("_DissolveAmount", 2f);

       
        StartCoroutine(PortalSequence());
    }

    private System.Collections.IEnumerator PortalSequence()
    {    yield return new WaitForSeconds(delayBeforeStart);
        
        yield return StartCoroutine(ChangeDissolve(1f, 0.341f, openDuration));

      
        yield return new WaitForSeconds(stayOpenTime);

   
        if (objectToSpawn)
        {
            Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        }

    
        yield return StartCoroutine(ChangeDissolve(0.347f, 2f, closeDuration));
    }

    private System.Collections.IEnumerator ChangeDissolve(float start, float end, float duration)
    {
        float t = 0f;
        while (t <duration)
        {
            t += Time.deltaTime;
            float val = Mathf.Lerp(start, end, t/duration);
            portalMat.SetFloat("_DissolveAmount", val);
            yield return null;
        }
        portalMat.SetFloat("_DissolveAmount", end);
    }
}
