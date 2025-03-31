using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private SceneLoader sceneLoader;

    [SerializeField]
    private MainMenuManager mainMenuManager;

    [SerializeField]
    private GamePlayManager gamePlayManager;

    [SerializeField]
    private CursorManager cursorManager;

    private static GameManager Instance;
    private static Stats gameStats;
    private SceneID initialScene = SceneID.LoadingScene;

    public static GameManager GetInstance()
    {
        return Instance;
    }

    public void Awake()
    {
        if (Instance)
        {
            MoveInstanceData();
        }
        else
        {
            Instance = this;
            InitStats();
            CalculateInitScene();
            LoadFirstScene();

            DontDestroyOnLoad(gameObject);
        }

        if (gamePlayManager)
        {
            gamePlayManager.SetUp();
        }
    }

    private void MoveInstanceData()
    {
        if (mainMenuManager)
            Instance.mainMenuManager = mainMenuManager;
        if (gamePlayManager)
            Instance.gamePlayManager = gamePlayManager;

        Destroy(gameObject);
    }

    public void InitStats()
    {
        gameStats.time = 0;
        gameStats.projectilesReflected = 0;
        gameStats.enemiesKilled = new();
    }

    private void CalculateInitScene()
    {
        if (sceneLoader)
            initialScene = SceneID.LoadingScene;
        else if (mainMenuManager)
            initialScene = SceneID.MainMenu;
        else if (gamePlayManager)
            initialScene = SceneID.GamePlay;
    }

    /// <summary>
    /// Load the main menu scene using the scene loader.
    /// In the case the game is initalized in the main menu or in the gamePlay scene,
    /// this functionality won't be used
    /// </summary>
    private void LoadFirstScene()
    {
        if (cursorManager)
            StartCoroutine(cursorManager.ChangeCursorTexture(GameState.Opening));

        switch (initialScene)
        {
            case SceneID.LoadingScene:
                SceneManager.LoadSceneAsync((int)SceneID.MainMenu, LoadSceneMode.Additive);
                break;
            default:
                break;
        }
    }

    public void LoadGame()
    {
        if (sceneLoader)
            sceneLoader.LoadGame();
        else
            ChangeScene((int)SceneID.GamePlay);
    }

    public void ReturnToMainMenu()
    {
        if (sceneLoader)
            sceneLoader.ReturnToMainMenu();
        else
            ChangeScene((int)SceneID.MainMenu);
    }

    public void ChangeScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public Stats GetGamestats() { return gameStats; }

    public void ChangeCursorTexture(GameState state)
    {
        if (!cursorManager)
            return;

        StartCoroutine(cursorManager.ChangeCursorTexture(state));
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
    Application.Quit();
#endif
    }
}
