using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseCanvas;

    [SerializeField]
    private Button continueGameButton;

    [SerializeField]
    private Button mainMenuButton;

    private GamePlayManager gamePlayManager;

    public void SetUp(GamePlayManager manager)
    {
        gamePlayManager = manager;
        continueGameButton.onClick.AddListener(delegate { ContinueGame(); });
        mainMenuButton.onClick.AddListener(delegate { ReturnToMainMenu(); });
    }

    private void OnEnable()
    {
        GamePlayManager.OnGamePaused += OnGamePaused;
    }

    private void OnDisable()
    {
        GamePlayManager.OnGamePaused -= OnGamePaused;
    }

    private void ContinueGame()
    {
        pauseCanvas.SetActive(false);
        gamePlayManager.ContinueGame();
    }

    private void OnGamePaused()
    {
        pauseCanvas.SetActive(true);
    }

    private void ReturnToMainMenu()
    {
        gamePlayManager.ReturnToMainMenu();
    }
}
