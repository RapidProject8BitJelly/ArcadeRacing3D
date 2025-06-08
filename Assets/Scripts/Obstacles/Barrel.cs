using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Barrel : MonoBehaviour
{
    [SerializeField] private PathFollower pathFollower;
    [SerializeField] private GameObject barrelModel;
    [SerializeField] private GameObject brokenBarrelModel;
    [SerializeField] private GameObject oils;
    [SerializeField] private Material brokenBarrelMaterial;
    
    [SerializeField] private float timeToRestartBarrel = 10f;
    [SerializeField] private float timeToAutoDisappearBarrel = 1000f;
    [SerializeField] private float randomSpread = 1.5f;
    [SerializeField] private float explosionForce = 30f;

    private readonly List<Vector3> _barrelPiecesStartPositions = new();
    private readonly List<Transform> _barrelPiecesTransforms = new();
    private readonly List<Rigidbody> _barrelPiecesRigidbodies = new(); 
    
    private Coroutine _autoDisappearBarrelCoroutine;
    
    private int randomOil; 
    private bool wasBarrelDestroyed;
    public string barrelID;
    
    private void Start()
    {
        InitializeBarrel();
    }
    
    private void InitializeBarrel()
    {
        barrelModel.SetActive(true);

        for (int i = 0; i < brokenBarrelModel.transform.childCount; i++)
        {
            var barrelPiece = brokenBarrelModel.transform.GetChild(i);
            _barrelPiecesStartPositions.Add(barrelPiece.localPosition);
            _barrelPiecesTransforms.Add(barrelPiece);
            _barrelPiecesRigidbodies.Add(barrelPiece.GetComponent<Rigidbody>());
        }
        
        brokenBarrelModel.SetActive(false);
        
        _autoDisappearBarrelCoroutine = StartCoroutine(AutoDisappearBarrel());
    }

    public void BarrelRoll()
    {
        float radius = barrelModel.transform.lossyScale.z / 2f;
        
        float circuit = 2 * Mathf.PI * radius;
        float rollAngle = (pathFollower.distance / circuit) * 360f;
            
        barrelModel.transform.DOLocalRotate(new Vector3(0, -rollAngle, 0), pathFollower.duration, 
            RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
    }
    
    public void DestroyBarrel()
    {
        if (!wasBarrelDestroyed)
        {
            wasBarrelDestroyed = true;
            pathFollower.StopFollowing();
            
            GetComponent<Collider>().enabled = false;
            
            barrelModel.SetActive(false);
            brokenBarrelModel.SetActive(true);
            
            ExplodeBarrel();

            randomOil = Random.Range(0, oils.transform.childCount);
            oils.transform.GetChild(randomOil).gameObject.SetActive(true);
            
            if(_autoDisappearBarrelCoroutine != null) StopCoroutine(_autoDisappearBarrelCoroutine);
            StartCoroutine(RestartBarrel());
        }
    }
    
    private IEnumerator AutoDisappearBarrel()
    {
        yield return new WaitForSeconds(timeToAutoDisappearBarrel);
        gameObject.SetActive(false);
    }

    private IEnumerator RestartBarrel()
    {
        yield return new WaitForSeconds(timeToRestartBarrel);
        
        wasBarrelDestroyed = false;
        GetComponent<Collider>().enabled = true;
        barrelModel.SetActive(true);
        
        oils.transform.GetChild(randomOil).gameObject.SetActive(false);

        pathFollower.StartFollowing();
        _autoDisappearBarrelCoroutine = StartCoroutine(AutoDisappearBarrel());
    }
    
    private void ExplodeBarrel()
    {
        for (int i = 0; i < _barrelPiecesTransforms.Count; i++)
        {
            var rb = _barrelPiecesRigidbodies[i];
            if (rb != null)
            {
                Vector3 explosionDir = (_barrelPiecesTransforms[i].position - transform.position).normalized;
                rb.AddForce((explosionDir + Random.insideUnitSphere * randomSpread) * explosionForce, ForceMode.Impulse);
            }
        }
        DisappearBarrelParts();
    }

    private void DisappearBarrelParts()
    {
        brokenBarrelMaterial.DOFade(0f, 3f).OnComplete(ResetBrokenBarrelModel);
    }

    private void ResetBrokenBarrelModel()
    {
        brokenBarrelModel.SetActive(false);
        
        var color = brokenBarrelMaterial.color;
        color.a = 1.0f;
        brokenBarrelMaterial.color = color;

        for (int i = 0; i < _barrelPiecesTransforms.Count; i++)
        {
            _barrelPiecesTransforms[i].localPosition = _barrelPiecesStartPositions[i];
        }
    }
}