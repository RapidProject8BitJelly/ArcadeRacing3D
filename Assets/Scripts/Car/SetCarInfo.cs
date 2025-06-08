using TMPro;
using UnityEngine;

public class SetCarInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI specialAbilityText;

    public void UpdateCarInfo(CarParameters carParameters)
    {
        speedText.text = "Max. speed: " + (carParameters.MaxSpeed*3.6f).ToString("0");
        specialAbilityText.text = "Power: " + carParameters.SpecialAbility.name;
    }
}
