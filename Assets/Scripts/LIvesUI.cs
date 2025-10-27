using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{
    public UFOController player;        // referencia al UFOController
    public Image heartPrefab;           // prefab del corazón (UI Image)
    public int maxHearts = 5;
    public float spacing = 300f;         // separación entre corazones

    private Image[] hearts;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player no asignado en LivesUI!");
            return;
        }
        if (heartPrefab == null)
        {
            Debug.LogError("Heart prefab no asignado en LivesUI!");
            return;
        }

        hearts = new Image[maxHearts];

        for (int i = 0; i < maxHearts; i++)
        {
            hearts[i] = Instantiate(heartPrefab, transform); // padre: heartsContainer
            hearts[i].rectTransform.localScale = Vector3.one; // asegurar escala correcta
            hearts[i].rectTransform.anchoredPosition = new Vector2(i * spacing, 0); // alineación horizontal
            hearts[i].gameObject.SetActive(false);
        }

        UpdateHearts();
    }

    void Update()
    {
        UpdateHearts();
    }

    void UpdateHearts()
    {
        if (player == null || hearts == null) return;

        int currentLives = Mathf.Clamp(player.lives, 0, maxHearts);

        for (int i = 0; i < maxHearts; i++)
        {
            if (hearts[i] != null)
                hearts[i].gameObject.SetActive(i < currentLives);
        }
    }
}
