using System;
using UnityEngine;
using static GamePlayManager;

public class GamePlayManager : MonoBehaviour
{
    public enum GameState 
    {
        Opening,
        Playing, 
        Pause,
        GameOver
    }

    #region Editor Variables

    [SerializeField]
    private EnemyManager enemyManager;

    [SerializeField]
    private HUDManager hudManager;
    
    [SerializeField]
    private CameraController cameraController;

    [Header("Game Rythm Speed")]
    [SerializeField]
    [Min(0)]
    private float minSpeed;

    [SerializeField]
    [Min(0)]
    private float maxSpeed;

    [SerializeField]
    private float speedIncreaseRate;

    #endregion

    #region Private Variables

    private float currentSpeed = 0;
    private float elapsedTime = 0f;
    private float currentGameTime = 0;
    private GameState gameState = GameState.Opening;

    #endregion

    #region Unity Callbacks

    void Awake()
    {
        hudManager.SetUp(this, 100);
        enemyManager.SetUp(this);
        cameraController.SetUp(this);
    }

    private void Update()
    {
        IncreaseGameRythmSpeed();
        IncreaseTime();
    }

    #endregion

    private void IncreaseGameRythmSpeed()
    {
        if (gameState != GameState.Playing || currentSpeed >= maxSpeed)
            return;

        elapsedTime += Time.deltaTime;

        float newSpeed = minSpeed + (maxSpeed - minSpeed) * (1 - Mathf.Exp(-speedIncreaseRate * elapsedTime));

        if (Mathf.Approximately(newSpeed, currentSpeed))
            return;

        currentSpeed = newSpeed;
    }

    private void IncreaseTime() 
    {
        if (gameState != GameState.Playing)
            return;

        currentGameTime += Time.deltaTime;
        hudManager.UpdateTime(currentGameTime);
    }

    /// <summary>
    /// Temporary function to start the game
    /// </summary>
    public void StartGame()
    {
        ChangeGameState(GameState.Playing);
        gameState = GameState.Playing;

        enemyManager.StartGame();
        currentSpeed = minSpeed;
    }

    public void GameOver()
    {
        ChangeGameState(GameState.GameOver);
        currentSpeed = 0;
        enemyManager.GameOver();
        hudManager.GameOver();
    }

    public void PauseGame() 
    {
    
    }

    private void ChangeGameState(GameState newState)
    {
        gameState = newState;
    }

    public GameState GetGameState() 
    {
        return gameState;
    }

    public void RestartGame()
    {
        GameManager.GetInstance().ChangeScene(1);
    }
}
