using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobbyPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField matchNameInput;
    [SerializeField] private TMP_Dropdown playerLimitDropdown;
    [SerializeField] private Button createButton;

    public string matchName;
    public int playerLimit;
    
    private void OnEnable()
    {
        matchNameInput.onValueChanged.AddListener(SetMatchName);
        playerLimitDropdown.onValueChanged.AddListener(SetPlayerLimit);
        createButton.interactable = playerLimitDropdown.value>0;
    }

    private void OnDisable()
    {
        matchNameInput.onValueChanged.RemoveAllListeners();
        playerLimitDropdown.onValueChanged.RemoveAllListeners();
    }
    
    private void SetMatchName(string value)
    {
        matchName = matchNameInput.text;
        CheckIfCanCreate();
    }
    
    private void SetPlayerLimit(int value)
    {
        playerLimit = playerLimitDropdown.value;
        CheckIfCanCreate();
    }

    private void CheckIfCanCreate()
    {
        if (matchName.Length >= 1 && playerLimit > 0)
        {
            createButton.interactable = true;
        }
        else
        {
            createButton.interactable = false;
        }
    }

    public void CreateLobby()
    {
        NetworkClient.Send(new CreateMatchMessage
        {
            matchName = matchNameInput.text,
            maxPlayers = (byte)(playerLimit+1),
        });
    }
}
