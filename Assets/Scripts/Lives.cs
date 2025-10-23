using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LivesHUD : MonoBehaviour
{
    public UFOController player;      // Referencia al UFO
    public Image heartPrefab;         // Prefab de un corazón
    public Transform heartsParent;    // Padre en el Canvas

    private List<Image> hearts = new List<Image>();

    void Start()
    {
        // Crear los corazones según el máximo de vidas
        for (int i = 0; i < player.maxLives; i++)
        {
            Image heart = Instantiate(heartPrefab, heartsParent);
            hearts.Add(heart);
        }
    }

    void Update()
    {
        // Activar/desactivar según las vidas actuales
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].enabled = i < player.lives;
        }
    }
}