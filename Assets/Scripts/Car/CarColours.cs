using UnityEngine;

[System.Serializable]
public struct CarColor
{
    public Color color;
    public Color emissionColor;
}

public class CarColours : MonoBehaviour
{
    [SerializeField] private CarColor[] colours;
    [SerializeField] private Color iconColor;
    [SerializeField] private Color additionalColor;

    public CarColor[] GetColours()
    {
        return colours; 
    }

    public Color GetIconColor()
    {
        return iconColor;
    }

    public Color GetAdditionalColor()
    {
        return additionalColor;
    }
}
