using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PathFollower : MonoBehaviour
{
    [SerializeField] private GameObject path;
    [SerializeField] private float speed;
    public bool isMoving;

    private void Start()
    {
        StartCoroutine(FollowPathCoroutine());
    }

    private IEnumerator FollowPathCoroutine()
    {
        isMoving = true;
        for (int i = 0; i < path.transform.childCount; i++)
        {
            Vector3 currentTarget = path.transform.GetChild(i).position;
            float distance = Vector3.Distance(transform.position, currentTarget);
            float duration = distance / speed;
            
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
            
            yield return transform.DOMove(currentTarget, duration).SetEase(Ease.Linear).WaitForCompletion();
        }
        isMoving = false;
    }
}
