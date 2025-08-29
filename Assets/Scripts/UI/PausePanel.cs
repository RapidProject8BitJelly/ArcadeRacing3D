using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    [SerializeField] private Button openPanelButton;
    [SerializeField] private Button resumeGameButton;
    [SerializeField] private Button backToMenuButton;

    [SerializeField] private GameObject pausePanel;

    private PlayersInputActions playersInputAction;

    private void OnEnable()
    {
        playersInputAction = new PlayersInputActions();
        playersInputAction.Enable();
        playersInputAction.UI.ChangePausePanelVisibility.performed += ChangePausePanelVisibilityWithKey;
        openPanelButton.onClick.AddListener(ChangePausePanelVisibility);
        resumeGameButton.onClick.AddListener(ChangePausePanelVisibility);
        backToMenuButton.onClick.AddListener(BackToMenu);
    }

    private void OnDisable()
    {
        playersInputAction.UI.ChangePausePanelVisibility.performed -= ChangePausePanelVisibilityWithKey;
        openPanelButton.onClick.RemoveAllListeners();
        resumeGameButton.onClick.RemoveAllListeners();
        backToMenuButton.onClick.RemoveAllListeners();
        playersInputAction.Disable();
    }

    private void ChangePausePanelVisibility()
    {
        pausePanel.SetActive(!pausePanel.activeSelf);
        if( pausePanel.activeSelf )
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
        
    }

    private void ChangePausePanelVisibilityWithKey(InputAction.CallbackContext ctx) {
        ChangePausePanelVisibility();
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
