using TMPro;
using UnityEngine;

public class PlayerNameTag : MonoBehaviour
{
    [SerializeField] private GameObject playerNicknameBackground;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private RaceProgressTracker raceProgressTracker;
    [SerializeField] private float nicknameTagPadding = 0.4f;

    private const float TagRotationCorrectionAngle = -90;
    
    private void Start()
    {
        SetPlayerNicknameTag();
    }

    private void LateUpdate()
    {
        if(Camera.main != null) transform.LookAt(Camera.main.transform);
        transform.Rotate(0, TagRotationCorrectionAngle, 0);
    }

    private void SetPlayerNicknameTag()
    {
        playerNameText.text = raceProgressTracker.playerNickname;
        playerNameText.ForceMeshUpdate();
        var scale = playerNicknameBackground.transform.localScale;
        scale.x = (playerNameText.textBounds.size.x) + nicknameTagPadding;
        playerNicknameBackground.transform.localScale = scale;
    }
}