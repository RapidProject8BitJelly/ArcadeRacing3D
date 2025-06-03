using UnityEngine;
using UnityEngine.Events;

public class TriggerRelay : MonoBehaviour
{
    [SerializeField] private UnityEvent<Collider> onTriggerEvent;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            onTriggerEvent?.Invoke(other);
        }
    }
}
