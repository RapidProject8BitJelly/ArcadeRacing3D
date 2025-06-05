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
    
    private readonly Color32 _indicatorsColor = new Color32(255, 255, 255, 255);
    private readonly Color32 _hiddenIndicatorsColor = new Color32(255, 255, 255, 0);

    private const float BaseTextSize = 62.4f;
    private const float HoverTextSize = 64.4f;


    private TMP_Text ButtonText
    {
        get
        {
            if (!_buttonText)
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
        
        if (ButtonText)
        {
            ButtonText.fontSize = HoverTextSize;
        }
    }
    
    public override void OnPointerExit(PointerEventData eventData)
    {
        foreach (var indicator in _indicators)
        {
            indicator.color = _hiddenIndicatorsColor;
        }
        
        if (ButtonText)
        {
            ButtonText.fontSize = BaseTextSize;
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
