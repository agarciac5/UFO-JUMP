using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = Vector3.zero;


    void Update()
    {
        if (player != null)
        {
            transform.position = player.position + offset;
            transform.rotation = player.rotation; 
        }
    }
}
