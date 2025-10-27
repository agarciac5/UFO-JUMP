using UnityEngine;
using System.Collections;

public class ResetTrailOnEnable : MonoBehaviour
{
    TrailRenderer tr;

    void Awake()
    {
        // Busca el TrailRenderer en este objeto o en sus hijos
        tr = GetComponent<TrailRenderer>();
        if (tr == null) tr = GetComponentInChildren<TrailRenderer>();
    }

    void OnEnable()
    {
        if (tr == null) return;

        // Apaga y limpia el trail al aparecer/reaparecer
        tr.emitting = false;
        tr.Clear();

        // Lo volvemos a encender en el siguiente frame
        StartCoroutine(ReenableNextFrame());
    }

    IEnumerator ReenableNextFrame()
    {
        yield return null; // espera 1 frame
        if (tr != null) tr.emitting = true;
    }

    void OnDisable()
    {
        if (tr != null) tr.Clear();
    }
}
