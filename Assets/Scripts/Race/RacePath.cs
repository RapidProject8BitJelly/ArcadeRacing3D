using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class RacePath : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;

    public static RacePath Instance { get; private set; }
    public Vector3[] waypoints;

    private void Awake()
    {
        Spline spline = splineContainer.Spline;
        waypoints = new Vector3[spline.Count];
        SetWaypoints(spline);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void SetWaypoints(Spline spline1)
    {
        for (int i = 0; i < spline1.Count; i++) {
            waypoints[i] = spline1[i].Position;
        }
    }

    public float GetPathLength()
    {
        float length = 0f;
        for (int i = 1; i < waypoints.Length; i++)
            length += Vector3.Distance(waypoints[i - 1], waypoints[i]);
        return length;
    }

    public float GetDistanceAlongPath(Vector3 position)
    {
        float distance = 0f;
        float closestDistance = float.MaxValue;
        float totalPath = 0f;

        Vector3 closestProjection = Vector3.zero;

        // Szukamy najlepszego dopasowania (najbliższego punktu na segmencie)
        for (int i = 1; i < waypoints.Length; i++)
        {
            Vector3 a = waypoints[i - 1];
            Vector3 b = waypoints[i];

            Vector3 projection = ProjectPointOnLineSegment(a, b, position);
            float distToPlayer = Vector3.Distance(projection, position);

            if (distToPlayer < closestDistance)
            {
                closestDistance = distToPlayer;
                closestProjection = projection;
                distance = totalPath + Vector3.Distance(a, projection);
            }

            totalPath += Vector3.Distance(a, b);
        }

        return distance;
    }


    private Vector3 ProjectPointOnLineSegment(Vector3 a, Vector3 b, Vector3 point)
    {
        Vector3 ab = b - a;
        float t = Vector3.Dot(point - a, ab.normalized) / ab.magnitude;
        t = Mathf.Clamp01(t);
        return a + ab * t;
    }
}
