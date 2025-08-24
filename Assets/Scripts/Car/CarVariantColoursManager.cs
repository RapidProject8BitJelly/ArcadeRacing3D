using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarVariantColoursManager : MonoBehaviour
{
    [SerializeField] private MeshRenderer carMesh;
    [SerializeField] private CarColours[] coloursSet;

    private List<CarColor> _currentColourSet = new();

    public void ChangeCurrentColourSet(int index)
    {
        _currentColourSet = coloursSet[index].GetColours().ToList();
        ChangeCarColours();
    }

    public CarColours[] GetColoursSet()
    {
        return coloursSet;
    }

    private void ChangeCarColours()
    {
        Material[] mats = carMesh.materials;

        for (int i = 0; i < mats.Length; i++) {
            mats[i].SetColor("_BaseColor", _currentColourSet[i].color);
            if (mats[i].IsKeywordEnabled("_EMISSION"))
            {
                mats[i].SetColor("_EmissionColor", _currentColourSet[i].emissionColor);
            }
        }
    }
}
