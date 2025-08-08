using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class PathFollower : MonoBehaviour
{
    [SerializeField] private GameObject path;
    [SerializeField] private UnityEvent actionDuringPathFollowing;
    
    [Min(0.01f)][SerializeField] private float speed;
    
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Coroutine pathCoroutine;
    private List<Transform> waypoints = new();
    
    public float distance;
    public float duration;
    
    private bool canFollow = true;

    private void Start()
    {
        SetupPathFollower();
    }

    private void SetupPathFollower()
    {
        for (int i = 0; i < path.transform.childCount; i++)
        {
            waypoints.Add(path.transform.GetChild(i));
        }
        startPosition = transform.position;
        startRotation = transform.rotation;
        pathCoroutine = StartCoroutine(FollowPathCoroutine());
    }
    
    private IEnumerator FollowPathCoroutine()
    {
        do
        { 
            transform.position = startPosition;
            transform.rotation = startRotation;
            
            for (int i = 0; i < waypoints.Count; i++)
            {
                Vector3 currentTarget = waypoints[i].position;
                distance = Vector3.Distance(transform.position, currentTarget);
                duration = distance / speed;
            
                Vector3 forwardDir;

                if (i < waypoints.Count - 1)
                {
                    Vector3 next = waypoints[i+1].position;
                    forwardDir = (next - transform.position).normalized;
                }
                else
                {
                    forwardDir = (currentTarget - transform.position).normalized;
                }
            
                forwardDir.y = 0;
            
                if (forwardDir != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(forwardDir, Vector3.up);
                    transform.DORotateQuaternion(targetRot, duration).SetEase(Ease.Linear);
                }

                actionDuringPathFollowing?.Invoke();

                yield return transform.DOMove(currentTarget, duration).SetEase(Ease.Linear).WaitForCompletion();
            }
        } while(canFollow);
    }

    public void StopFollowing()
    {
        if (pathCoroutine != null)
        {
            StopCoroutine(pathCoroutine);
            pathCoroutine = null;
        }
    }

    public void StartFollowing()
    {
        if (pathCoroutine != null)
        {
            StopCoroutine(pathCoroutine);
        }
        pathCoroutine = StartCoroutine(FollowPathCoroutine());
    }
}
