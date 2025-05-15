using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PathFollower : MonoBehaviour
{
    [SerializeField] private GameObject path;

    private void Start()
    {
        StartCoroutine(FollowPathCoroutine());
    }

    private IEnumerator FollowPathCoroutine()
    {
        for (int i = 0; i < path.transform.childCount; i++)
        {
            transform.DOLookAt(path.transform.GetChild(i).position, 0.5f, AxisConstraint.Y, Vector3.up).SetEase(Ease.Linear);
            yield return transform.DOMove(path.transform.GetChild(i).position, 0.5f).SetEase(Ease.Linear)
                .WaitForCompletion();
        }
    }
    
}
