using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lapCounter;
    [SerializeField] private TextMeshProUGUI announcementBoard;

    public void IncreaseLapCounter(int lapNumber, int LAPS)
    {
        lapCounter.text = "LAP: " + lapNumber + "/" + LAPS;
    }

    public void SetAnnouncementBoardText(string text)
    {
        announcementBoard.text = text;
    }
}
