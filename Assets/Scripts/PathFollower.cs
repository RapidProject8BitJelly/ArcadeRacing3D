using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PathFollower : MonoBehaviour
{
    [SerializeField] private GameObject path;
    [SerializeField] private float speed;
    [SerializeField] private GameObject barrelModel;
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
            
            float radius = barrelModel.transform.lossyScale.z * 0.5f; // zakładamy że toczy się po osi X

            // 🔁 Oblicz obrót wokół osi X (toczenie)
            float obwod = 2 * Mathf.PI * radius;
            float rollAngle = (distance / obwod) * 360f;

            // 🔄 Dodaj lokalny obrót toczenia
            
            
            barrelModel.transform.DOLocalRotate(new Vector3(rollAngle, 0, 90), duration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);

            
            yield return transform.DOMove(currentTarget, duration).SetEase(Ease.Linear).WaitForCompletion();
        }
        isMoving = false;
    }
}
