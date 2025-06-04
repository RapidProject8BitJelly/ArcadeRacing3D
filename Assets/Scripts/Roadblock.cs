using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Roadblock : MonoBehaviour
{
    [SerializeField] private bool dynamicRoadblock;
    [SerializeField] private float moveToWaypointDuration;
    
    [SerializeField] private GameObject leftWaypoint;
    [SerializeField] private GameObject rightWaypoint;

    private readonly bool _canMove = true;

    private void Start()
    {
        if (dynamicRoadblock)
        {
            StartCoroutine(RoadblockMove());
        }
    }

    private IEnumerator RoadblockMove()
    {
        do
        {
            yield return transform.DOMove(leftWaypoint.transform.position, moveToWaypointDuration).SetEase(Ease.Linear).WaitForCompletion();
            yield return transform.DOMove(rightWaypoint.transform.position, moveToWaypointDuration).SetEase(Ease.Linear).WaitForCompletion();
        } while(_canMove);
    }
}