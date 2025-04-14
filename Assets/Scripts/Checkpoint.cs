using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject teleportPosition;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<CarCheckpointController>().CheckPointVisited(this);
        }
    }
}
