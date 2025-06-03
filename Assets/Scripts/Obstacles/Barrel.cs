using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mirror;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Barrel : MonoBehaviour
{
    [SerializeField] private PathFollower pathFollower;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private GameObject barrelModel;
    [SerializeField] private GameObject brokenBarrelModel;
    [SerializeField] private GameObject oils;
    [SerializeField] private float timeToRestartBarrel;
    [SerializeField] private float timeToDisappearBarrel;
    [SerializeField] private Material brokenBarrelMaterial;
    
    private Coroutine disappearBarrel;
    private bool wasBarrelDestroyed;
    private List<Vector3> barrelPartsPosition = new();
    
    public string barrelID;
    private int randomOil;
    
    private void Start()
    {
        barrelModel.SetActive(true);

        for (int i = 0; i < brokenBarrelModel.transform.childCount; i++)
        {
            barrelPartsPosition.Add(brokenBarrelModel.transform.GetChild(i).localPosition);
        }
        
        brokenBarrelModel.SetActive(false);
        
        disappearBarrel = StartCoroutine(DisappearBarrel());
        oils.transform.SetParent(gameObject.transform);
        for (int i = 0; i < oils.transform.childCount; i++)
        {
            oils.transform.GetChild(i).gameObject.SetActive(false);
        }
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
        if (!wasBarrelDestroyed)
        {
            GetComponent<Collider>().enabled = false;
            brokenBarrelModel.SetActive(true);
            DisappearBarrelParts();
            wasBarrelDestroyed = true;
            pathFollower.StopFollowing();
            randomOil = UnityEngine.Random.Range(0, oils.transform.childCount);
            oils.transform.GetChild(randomOil).gameObject.SetActive(true);
            barrelModel.SetActive(false);
            if(disappearBarrel != null) StopCoroutine(disappearBarrel);
            StartCoroutine(RestartBarrel());
        }
    }

    private IEnumerator RestartBarrel()
    {
        yield return new WaitForSeconds(timeToRestartBarrel);
        wasBarrelDestroyed = false;
        oils.transform.GetChild(randomOil).gameObject.SetActive(false);
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

    private void DisappearBarrelParts()
    {
        for (int i = 0; i < brokenBarrelModel.transform.childCount; i++)
        {
            var rb = brokenBarrelModel.transform.GetChild(i).gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 explosionDir = (brokenBarrelModel.transform.GetChild(i).gameObject.transform.position - transform.position).normalized;
                rb.AddForce((explosionDir + Random.insideUnitSphere * 1.5f) * 30, ForceMode.Impulse);
            }
        }
        for (int i = 0; i < brokenBarrelModel.transform.childCount; i++)
        {
            //brokenBarrelModel.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("NoPlayerCollision");
        }
        brokenBarrelMaterial.DOFade(0f, 3f).OnComplete(kakaka);
    }

    private void kakaka()
    {
        brokenBarrelModel.SetActive(false);
        var color = brokenBarrelMaterial.color;
        color.a = 1.0f;
        brokenBarrelMaterial.color = color;

        for (int i = 0; i < brokenBarrelModel.transform.childCount; i++)
        {
            brokenBarrelModel.transform.GetChild(i).localPosition = barrelPartsPosition[i];
            brokenBarrelModel.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Default");
        }
        
    }
}
