using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : Button, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private List<Image> _indicators = new List<Image>();
    
    private TMP_Text _buttonText;
    private Color32 _indicatorsColor = new Color32(255, 255, 255, 255);
    private Color32 _hiddenIndicatorsColor = new Color32(255, 255, 255, 0);
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

    public override void OnPointerEnter(PointerEventData eventData)
    {
        foreach (var indicator in _indicators)
        {
            indicator.color = _indicatorsColor;
        }
    }
    
    public override void OnPointerExit(PointerEventData eventData)
    {
        foreach (var indicator in _indicators)
        {
            indicator.color = _hiddenIndicatorsColor;
        }
    }
}

[CustomEditor(typeof(MenuButton))]
public class MenuButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}
