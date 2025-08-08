using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuButton : Button, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private List<Image> _indicators = new List<Image>();
    [SerializeField] private ActionType _actionType = ActionType.None;
    [SerializeField] private RectTransform _optionsPanel;
    [SerializeField] private RectTransform _creditsPanel;
    [SerializeField] private Vector2 _openPosition;
    
    private enum ActionType
    {
        None,
        StartGame,
        Options,
        Credits,
        Exit
    }
    
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

    protected override void Start()
    {
        base.Start();
        onClick.AddListener(Submit);
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        onClick.RemoveListener(Submit);
    }

    private void Submit()
    {
        switch (_actionType)
        {
            case ActionType.StartGame:
                SceneManager.LoadScene("Lobby");
                break;
            case ActionType.Options:
                if (_optionsPanel != null)
                {
                    _optionsPanel.gameObject.SetActive(true);
                    _optionsPanel.DOAnchorPos(_openPosition, .5f).SetEase(Ease.OutBack);
                }
                break;
            case ActionType.Credits:
                if (_creditsPanel != null)
                {
                    _creditsPanel.gameObject.SetActive(true);
                    _creditsPanel.DOAnchorPos(_openPosition, .5f).SetEase(Ease.OutBack);
                }
                break;
            case ActionType.Exit:
                #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
                #endif
                Application.Quit();
                break;
            default:
                Debug.LogWarning("No action assigned to this button.");
                break;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MenuButton))]
public class MenuButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}
#endif