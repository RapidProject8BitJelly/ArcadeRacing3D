using TMPro;
using UnityEngine.UI;

public class MenuButton : Button
{
    private TMP_Text _buttonText;
    public TMP_Text ButtonText
    {
        get
        {
            if (_buttonText == null)
            {
                _buttonText = GetComponentInChildren<TMP_Text>();
            }
            return _buttonText;
        }
    }
}
