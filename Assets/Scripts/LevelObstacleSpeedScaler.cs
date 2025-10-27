using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class LevelObstacleSpeedScaler : MonoBehaviour
{
    public string obstacleTag = "Obstacle";     // meteoritos
    public string[] alsoAffectTags = { "Shield" }; // p.ej. ShieldPickUp
    public float multiply = 1.6f;               // súbelo para probar (2.0) y luego ajusta
    public float add = 0f;
    public float rescanEverySeconds = 0.4f;
    public bool debugLogs = false;

    readonly HashSet<Object> applied = new HashSet<Object>();
    float timer;

    // nombres posibles del componente de movimiento
    static readonly string[] moverTypeNamesMain = { "ObstacleMove", "ObstacleMover" };
    static readonly string[] moverTypeNamesExtra = { "ItemMove", "PickupMove", "ShieldMove", "ShieldMover" };

    // posibles nombres del campo/prop de velocidad
    static readonly string[] speedNames = { "Speed","speed","moveSpeed","MoveSpeed","_speed","currentSpeed","_currentSpeed" };

    void OnEnable() { ScanAndApply(); }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= rescanEverySeconds)
        {
            ScanAndApply();
            timer = 0f;
        }
    }

    void ScanAndApply()
    {
        // 1) recolecta objetivos por tag
        List<GameObject> targets = new List<GameObject>();

        AddByTag(targets, obstacleTag);
        if (alsoAffectTags != null)
            for (int i = 0; i < alsoAffectTags.Length; i++)
                if (!string.IsNullOrEmpty(alsoAffectTags[i])) AddByTag(targets, alsoAffectTags[i]);

        // 2) intenta escalar su "Speed" en el componente de movimiento
        for (int i = 0; i < targets.Count; i++)
        {
            var go = targets[i];
            if (!go) continue;

            Component mover = FindMoverComponent(go);
            if (mover == null) continue;
            if (applied.Contains(mover)) continue;

            if (TryScaleSpeed(mover, out float before, out float after))
            {
                applied.Add(mover);
                if (debugLogs) Debug.Log($"[SpeedScaler] {go.name} {mover.GetType().Name}: {before} -> {after}");
            }
            else if (debugLogs)
            {
                Debug.Log($"[SpeedScaler] No encontré campo/prop de velocidad en {go.name} ({mover.GetType().Name})");
            }
        }
    }

    void AddByTag(List<GameObject> list, string tag)
    {
        if (string.IsNullOrEmpty(tag)) return;
        GameObject[] arr = GameObject.FindGameObjectsWithTag(tag);
        if (arr == null) return;
        for (int i = 0; i < arr.Length; i++)
            if (arr[i]) list.Add(arr[i]);
    }

    Component FindMoverComponent(GameObject go)
    {
        // prueba nombres principales (meteoritos)
        for (int i = 0; i < moverTypeNamesMain.Length; i++)
        {
            var c = go.GetComponent(moverTypeNamesMain[i]);
            if (c) return c;
        }
        // prueba nombres extra (pickups)
        for (int i = 0; i < moverTypeNamesExtra.Length; i++)
        {
            var c = go.GetComponent(moverTypeNamesExtra[i]);
            if (c) return c;
        }
        // fallback: cualquier MonoBehaviour cuyo tipo contenga "Move"
        var all = go.GetComponents<MonoBehaviour>();
        for (int i = 0; i < all.Length; i++)
        {
            var mb = all[i];
            if (!mb) continue;
            string n = mb.GetType().Name;
            if (n.ToLower().Contains("move")) return mb;
        }
        return null;
    }

    bool TryScaleSpeed(Component comp, out float before, out float after)
    {
        before = 0f; after = 0f;
        var ty = comp.GetType();

        // campos
        for (int i = 0; i < speedNames.Length; i++)
        {
            var f = ty.GetField(speedNames[i], BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);
            if (f != null && f.FieldType == typeof(float))
            {
                before = (float)f.GetValue(comp);
                after = before * multiply + add;
                f.SetValue(comp, after);
                return true;
            }
        }
        // propiedades
        for (int i = 0; i < speedNames.Length; i++)
        {
            var p = ty.GetProperty(speedNames[i], BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);
            if (p != null && p.CanRead && p.CanWrite && p.PropertyType == typeof(float))
            {
                before = (float)p.GetValue(comp, null);
                after = before * multiply + add;
                p.SetValue(comp, after, null);
                return true;
            }
        }
        return false;
    }
}
