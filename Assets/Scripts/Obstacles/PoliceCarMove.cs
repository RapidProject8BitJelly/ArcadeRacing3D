using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;

public class PoliceCarMove : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private GameObject player;
    [SerializeField] private float maxRightOffset;
    [SerializeField] private float maxLeftOffset;
    [SerializeField] private float distanceToPlayer;

    [SerializeField] private List<Vector3> waypoints = new();
    private List<Vector3> waypointsStartPositions = new();
    private Spline trackPathSpline;

    private float speed = 10;
    private float distance;
    private float duration;
    private bool canFollow;
    
    private int currentWaypoint = 0;
    private Tween goToWaypointTween;
    
    private void Start()
    {
        var vector3 = transform.position;
        vector3.z = player.transform.position.z + distanceToPlayer;
        transform.position = vector3;
        
        waypoints = GetPathPoints();
        currentWaypoint = GetNearestKnotIndexToPosition(transform.position);
        StartCoroutine(MoveWithDynamicSpeedAndRotation());
        //StartCoroutine(GoToNextPoint());
    }
    
    private void Update()
    {
        int nearestPoint = GetNearestKnotIndexToPosition(player.transform.position);
        
        Vector3 refPoint = waypointsStartPositions[nearestPoint];
        Vector3 refForward = (waypointsStartPositions[nearestPoint + 1] - refPoint).normalized;
        Vector3 refRight = Vector3.Cross(Vector3.up, refForward);
        
        float pathOffset = Vector3.Dot(player.transform.position - refPoint, refRight);
        
        for (int i = nearestPoint; i < waypoints.Count - 1; i++)
        {
            Vector3 basePos = waypointsStartPositions[i];
            Vector3 forward = (waypointsStartPositions[i + 1] - basePos).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, forward);
            
            float clampedOffset = Mathf.Clamp(pathOffset, maxLeftOffset, maxRightOffset);
            
            waypoints[i] = basePos + right * clampedOffset;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(goToWaypointTween != null) goToWaypointTween.Kill();
        }
    }
    
    private List<Vector3> GetPathPoints()
    {
        trackPathSpline = splineContainer[0];
        List<Vector3> pathPoints = new();
        float length = trackPathSpline.Count;

        for (int i = 0; i < length; i++)
        {
            pathPoints.Add(trackPathSpline[i].Position);
            waypointsStartPositions.Add(trackPathSpline[i].Position);
        }
        return pathPoints;
    }
    
    private int GetNearestKnotIndexToPosition(Vector3 playerPosition, Transform splineTransform = null)
    {
        int closestIndex = -1;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < trackPathSpline.Count; i++)
        {
            Vector3 knotPos = trackPathSpline[i].Position;
            
            if (splineTransform != null)
                knotPos = splineTransform.TransformPoint(knotPos);

            float distance = Vector3.Distance(playerPosition, knotPos);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }
    
    private IEnumerator MoveWithDynamicSpeedAndRotation()
    {
        while (Vector3.Distance(transform.position, waypoints[currentWaypoint]) > 0.01f)
        {
            float dynamicSpeed = player.GetComponent<Rigidbody>().linearVelocity.magnitude;
            // 1. Przesuń w stronę celu
            Vector3 nextPosition = Vector3.MoveTowards(
                transform.position,
                waypoints[currentWaypoint],
                dynamicSpeed * Time.deltaTime
            );

            // 2. Oblicz kierunek jazdy
            Vector3 direction = (nextPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }

            transform.position = nextPosition;

            yield return null;
        }

        if (currentWaypoint <=waypoints.Count - 1)
        {
            currentWaypoint++;
            StartCoroutine(MoveWithDynamicSpeedAndRotation());
        }
        
    }

}


// prędkość pojazdu zawsze równa prędkości gracza
// jeżeli >0 jedź do następnego punktu, ale jeśli w trakcie zmieni się prędkość na zero to stop
// powtórz do końca trasy xD