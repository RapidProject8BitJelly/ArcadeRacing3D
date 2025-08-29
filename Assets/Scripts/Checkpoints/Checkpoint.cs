using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject teleportPosition;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player");
            other.gameObject.GetComponent<CarCheckpointController>().CheckPointVisited(this);
        }
    }
}
