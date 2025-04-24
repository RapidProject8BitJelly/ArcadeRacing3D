using System;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class MatchGUI : MonoBehaviour
{
    Guid matchId;

    [Header("GUI Elements")]
    public Image image;
    public Toggle toggleButton;
    public Text matchName;
    public Text playerCount;
    public Image lobbyAvailableImage;
    
    [SerializeField] private Color lobbyAvailableColor;
    [SerializeField] private Color lobbyUnavailableColor;

    [Header("Diagnostics")]
    [ReadOnly, SerializeField] internal CanvasController canvasController;

    public void Awake()
    {
        canvasController = GameObject.FindObjectOfType<CanvasController>();
    }

    [ClientCallback]
    public void OnToggleClicked(bool isOn)
    {
        canvasController.SelectMatch(isOn ? matchId : Guid.Empty);
        image.color = isOn ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 1f, 1f, 0.2f);
    }

    [ClientCallback]
    public Guid GetMatchId() => matchId;

    [ClientCallback]
    public void SetMatchInfo(MatchInfo infos)
    {
        matchId = infos.matchId;
        matchName.text = infos.matchName;
        playerCount.text = $"{infos.players} / {infos.maxPlayers}";
        if (infos.players < infos.maxPlayers)
        {
            lobbyAvailableImage.color = lobbyAvailableColor;
        }
        else
        {
            lobbyAvailableImage.color = lobbyUnavailableColor;
        }
    }
}