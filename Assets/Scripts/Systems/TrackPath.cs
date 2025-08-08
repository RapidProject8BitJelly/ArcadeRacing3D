using System;
using Unity.VisualScripting;
using UnityEngine;


public class TrackPath : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float maxLeftDistance = 1f;
    [SerializeField] private float maxRightDistance = 1f;

    private float centerX;

    private void Start()
    {
        // Ustawiamy środek toru RAZ na starcie (jeśli tor się nie przesuwa)
        centerX = transform.position.x;
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        Vector3 pos = transform.position;

        float playerX = player.transform.position.x;
        float offsetFromCenter = playerX - centerX;

        // DEBUG: pokaż offset
        Debug.Log($"Offset: {offsetFromCenter}");

        // Ogranicz przesunięcie względem środka toru
        float clampedOffset = Mathf.Clamp(offsetFromCenter, -maxLeftDistance, maxRightDistance);

        pos.x = centerX + clampedOffset;

        transform.position = pos;
    }
}