using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    [SerializeField] private PathFollower pathFollower;
    [SerializeField] private float rotationSpeed = 10f;

    private void Update()
    {
        if (pathFollower.isMoving)
        {
            //transform.Rotate(Vector3.right, -1f * rotationSpeed);
        }
    }
}
