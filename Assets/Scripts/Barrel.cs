using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

public class Barrel : MonoBehaviour
{
    [SerializeField] private PathFollower pathFollower;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private GameObject barrelModel;
    [SerializeField] private GameObject oils;
    [SerializeField] private float timeToRestartBarrel;
    [SerializeField] private float timeToDisappearBarrel;

    private Coroutine disappearBarrel;
    
    public string barrelID;
    private int randomOil;
    
    private void Start()
    {
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
        pathFollower.StopFollowing();
        randomOil = UnityEngine.Random.Range(0, oils.transform.childCount);
        Debug.Log(randomOil);
        oils.transform.GetChild(randomOil).gameObject.SetActive(true);
        barrelModel.SetActive(false);
        GetComponent<Collider>().enabled = false;
        if(disappearBarrel != null) StopCoroutine(disappearBarrel);
        StartCoroutine(RestartBarrel());
    }

    private IEnumerator RestartBarrel()
    {
        yield return new WaitForSeconds(timeToRestartBarrel);
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
}
