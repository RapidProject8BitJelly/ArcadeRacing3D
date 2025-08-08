using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class SplineGenerator : MonoBehaviour
{
    public bool generateOnStart = false;
    [SerializeField] private GameObject splinePoints;

    private void Start()
    {
        #if UNITY_EDITOR
            if (generateOnStart) 
                GenerateSpline();
        #endif
    }

    #if UNITY_EDITOR
    [ContextMenu("Generate Spline From Children")]
    public void GenerateSpline()
    {
        SplineContainer container = GetComponent<SplineContainer>();
        if (container == null)
            container = gameObject.AddComponent<SplineContainer>();

        var spline = new Spline();

        for (int i = 0; i < splinePoints.transform.childCount; i++)
        {
            var child = splinePoints.transform.GetChild(i);
            
            spline.Add(new BezierKnot(child.GetComponent<CenterPoint>().GetCenterPoint().position));
        }

        container.Spline = spline;

        Debug.Log($"[SplineGenerator] Utworzono spline z {spline.Count} punktów!", this);
    }
#endif
}