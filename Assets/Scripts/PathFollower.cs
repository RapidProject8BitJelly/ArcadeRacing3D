using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class PathFollower : MonoBehaviour
{
    [SerializeField] private GameObject path;
    [SerializeField] private float speed;
    [SerializeField] private GameObject barrelModel;
    [SerializeField] private UnityEvent action;
    public float distance;
    public float duration;
    private Vector3 startPosition;
    private Vector3 startRotation;
    private Coroutine pathCoroutine;

    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
        pathCoroutine = StartCoroutine(FollowPathCoroutine());
    }

    private IEnumerator FollowPathCoroutine()
    {
        transform.position = startPosition;
        var rotation = transform.rotation;
        rotation.eulerAngles = startRotation;
        transform.rotation = rotation;
        
        for (int i = 0; i < path.transform.childCount; i++)
        {
            Vector3 currentTarget = path.transform.GetChild(i).position;
            distance = Vector3.Distance(transform.position, currentTarget);
            duration = distance / speed;
            
            Vector3 forwardDir;

            if (i < path.transform.childCount - 1)
            {
                Vector3 next = path.transform.GetChild(i + 1).position;
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

            action?.Invoke();

            yield return transform.DOMove(currentTarget, duration).SetEase(Ease.Linear).WaitForCompletion();
        }
        
        pathCoroutine = StartCoroutine(FollowPathCoroutine());
    }

    public void StopFollowing()
    {
        StopCoroutine(pathCoroutine);
    }

    public void StartFollowing()
    {
        pathCoroutine = StartCoroutine(FollowPathCoroutine());
    }
}
