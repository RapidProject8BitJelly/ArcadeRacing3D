using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAudio : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    [SerializeField] private Sound hoverSound = Sound.ButtonHover;
    [SerializeField] private Sound clickSound = Sound.UiClick;
    
    private bool _isMouseOver = false;
    private Button _targetButton;

    private void Awake()
    {
        _targetButton = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!_targetButton.interactable) return;
        
        SoundManager.instance.Play(hoverSound);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(!_targetButton.interactable) return;
        
        SoundManager.instance.Play(clickSound);
    }
}
