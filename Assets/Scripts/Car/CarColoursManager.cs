using UnityEngine;

public class CarColoursManager : MonoBehaviour
{
    [SerializeField] private CarVariantColoursManager[] carVariantColoursManagers;

    private CarVariantColoursManager _currentCarVariant;

    private void Awake()
    {
        _currentCarVariant = carVariantColoursManagers[0];
        ChangeCarColour(0);
    }

    public CarVariantColoursManager ChangeCurrentCarVariant(int index)
    {
        _currentCarVariant = carVariantColoursManagers[index];
        ChangeCarColour(0);
        return _currentCarVariant;
    }

    public void ChangeCarColour(int index)
    {
        _currentCarVariant.ChangeCurrentColourSet(index);
    }
}
