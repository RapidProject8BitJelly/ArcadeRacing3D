using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mirror;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    [SerializeField] private PathFollower pathFollower;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private GameObject barrelModel;
    [SerializeField] private GameObject oil;
    [SerializeField] private float timeToRestartBarrel;
    [SerializeField] private float timeToDisappearBarrel;

    private Coroutine disappearBarrel;
    
    public string barrelID;
    
    private void Start()
    {
        disappearBarrel = StartCoroutine(DisappearBarrel());
        oil.transform.SetParent(gameObject.transform);
        oil.SetActive(false);
    }

    public void BarrelRotation()
    {
        float radius = barrelModel.transform.lossyScale.z * 0.5f; 
        
        float obwod = 2 * Mathf.PI * radius;
        float rollAngle = (pathFollower.distance / obwod) * 360f;
            
        barrelModel.transform.DOLocalRotate(new Vector3(0, -rollAngle, 0), pathFollower.duration, 
            RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
    }
    
    public void DestroyBarrel()
    {
        oil.SetActive(true);
        pathFollower.StopFollowing();
        barrelModel.SetActive(false);
        GetComponent<Collider>().enabled = false;
        if(disappearBarrel != null) StopCoroutine(disappearBarrel);
        StartCoroutine(RestartBarrel());
    }

    private IEnumerator RestartBarrel()
    {
        yield return new WaitForSeconds(timeToRestartBarrel);
        oil.SetActive(false);
        barrelModel.SetActive(true);
        GetComponent<Collider>().enabled = true;
        pathFollower.StartFollowing();
        disappearBarrel = StartCoroutine(DisappearBarrel());
    }

    private IEnumerator DisappearBarrel()
    {
        yield return new WaitForSeconds(timeToDisappearBarrel);
        gameObject.SetActive(false);
    }
}
