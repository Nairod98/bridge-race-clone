using UnityEngine;

public class Teleporter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            other.transform.position = GameObject.Find("Floor" + playerMovement.actualFloor).transform.position;
        }

        if (other.gameObject.CompareTag("AI"))
        {
            AIBehaviour aiBehaviour = other.GetComponent<AIBehaviour>();
            other.transform.position = GameObject.Find("Floor" + aiBehaviour.actualFloor).transform.position;
            StartCoroutine(aiBehaviour.FindBrick());
        }
    }
}
