using System;
using TMPro;
using UnityEngine;

public class PlayerNameTag : MonoBehaviour
{
    [SerializeField] private GameObject playerNicknameBackground;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private RaceProgressTracker raceProgressTracker;

    private void Start()
    {
        playerNameText.text = raceProgressTracker.playerNickname;
        playerNameText.ForceMeshUpdate();
        var scale = playerNicknameBackground.transform.localScale;
        scale.x = (playerNameText.textBounds.size.x) + 0.4f;
        playerNicknameBackground.transform.localScale = scale;
    }

    private void LateUpdate()
    {
        gameObject.transform.LookAt(Camera.main.transform);
        transform.Rotate(0, -90, 0);
    }
}
