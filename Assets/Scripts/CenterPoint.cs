using UnityEngine;

public class CenterPoint : MonoBehaviour
{
    [SerializeField] private GameObject centerPoint;
    public Vector3 GetCenterPoint()
    {
        return centerPoint.transform.position;
    }
}
