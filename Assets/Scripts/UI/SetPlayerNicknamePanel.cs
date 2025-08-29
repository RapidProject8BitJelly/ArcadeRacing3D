using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetPlayerNicknamePanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField nicknameInput;
    [SerializeField] private Button applyButton;
    [SerializeField] private PlayerGUI playerGUI;

    public string playerNickname;

    private void OnEnable()
    {
        applyButton.onClick.AddListener(SetPlayerNickname);
    }

    private void OnDisable()
    {
        applyButton.onClick.RemoveAllListeners();
    }

    private void SetPlayerNickname()
    {
        string nickname = nicknameInput.text;
        playerGUI.SetPlayerName(nickname);
        playerNickname = nickname;
    }
}
