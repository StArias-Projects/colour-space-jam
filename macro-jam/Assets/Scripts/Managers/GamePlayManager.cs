using System;
using UnityEngine;

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
    private PlayerManager playerManager;

    [SerializeField]
    private EnemyManager enemyManager;

    [SerializeField]
    private HUDManager hudManager;
    
    [SerializeField]
    private ProjectileManager projectileManager;
    
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

    public static Action OnGameOver;

    #region Unity Callbacks

    void Awake()
    {
        playerManager.SetUp(this);
        hudManager.SetUp(this, playerManager.GetMaxHealth());
        enemyManager.SetUp(this, projectileManager, playerManager.transform);
        projectileManager.SetUp();
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

        OnGameOver?.Invoke();
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

    public void UpdateHealth(float health)
    {
        hudManager.UpdateHealth(health);
    }
}
