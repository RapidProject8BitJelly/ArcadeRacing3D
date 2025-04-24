using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNicknamePanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField nicknameInput;
    [SerializeField] private Selectable[] uiElements;
    [SerializeField] private Image[] uiPanels;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color nonInteractableColor;

    private void OnEnable()
    {
        nicknameInput.onValueChanged.AddListener(OnNicknameChanged);
    }

    private void OnNicknameChanged(string value)
    {
        bool canInteract = !string.IsNullOrEmpty(value);
        foreach (Selectable uiElement in uiElements)
        {
            uiElement.interactable = canInteract;
        }

        foreach (Image panel in uiPanels)
        {
            panel.color = canInteract ? normalColor: nonInteractableColor;
        }
    }

    public void SetNickInMatch()
    {
        NetworkClient.Send(new SetPlayerNickname
        {
            nickname = nicknameInput.text
        });
    }
}
