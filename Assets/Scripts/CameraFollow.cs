using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;    

    void Update()
    {
        transform.position = player.transform.position + new Vector3(0, 4.3f, -3);
    }
}
