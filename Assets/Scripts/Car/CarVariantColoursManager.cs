using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarVariantColoursManager : MonoBehaviour
{
    [SerializeField] private MeshRenderer carMesh;
    [SerializeField] private MeshRenderer[] additionalMeshes;
    [SerializeField] private SkinnedMeshRenderer[] additionalSkinnedMeshes;
    [SerializeField] private int[] additionalMaterialsIndex;
    [SerializeField] private CarColours[] coloursSet;

    private List<CarColor> _currentColourSet = new();
    private CarColours _currentColours;

    public void ChangeCurrentColourSet(int index)
    {
        _currentColours = coloursSet[index];
        _currentColourSet = _currentColours.GetColours().ToList();
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

        if (additionalMeshes != null)
        {
            for (int i = 0; i < additionalMeshes.Length; i++)
            {
                Material[] material = additionalMeshes[i].materials;
                material[additionalMaterialsIndex[i]].SetColor("_BaseColor", _currentColours.GetAdditionalColor());
            }
        }

        if(additionalSkinnedMeshes != null)
        {
            for(int i = 0;i < additionalSkinnedMeshes.Length;i++)
            {
                Material[] material = additionalSkinnedMeshes[i].materials;
                material[additionalMaterialsIndex[i]].SetColor("_BaseColor", _currentColours.GetAdditionalColor());
            }
        }
    }
}
