using System;
using System.Collections;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
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

    [SerializeField]
    private VFXManager vfxManager;

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
    private Stats gameStats;
    private GameManager gameManager;

    public static event Action OnGameOver;
    #endregion

    #region Unity Callbacks

    private void Update()
    {
        if (gameState != GameState.Playing)
            return;

        IncreaseGameRythmSpeed();
        IncreaseTime();
    }

    #endregion

    #region Game States

    public void SetUp()
    {
        gameManager = GameManager.GetInstance();
        gameStats.enemiesKilled = new();

        playerManager.SetUp(this);
        hudManager.SetUp(this, playerManager.GetMaxHealth());
        enemyManager.SetUp(this, projectileManager, playerManager.transform);
        projectileManager.SetUp(enemyManager,vfxManager);
        cameraController.SetUp(this);
        vfxManager.SetUp(this, enemyManager);

        StartGame();
    }

    private void StartGame()
    {
        gameState = GameState.Playing;
        currentSpeed = minSpeed;
    }

    public IEnumerator GameOver()
    {
        gameState = GameState.GameOver;

        gameStats.time = (uint)currentGameTime;
        currentSpeed = 0;

        playerManager.StartDeathAnimation();
        OnGameOver?.Invoke();
        
        yield return new WaitUntil(() => playerManager.IsDeathAnimationFinished());

        gameManager.GameOver(gameStats);
    }

    public void ResetGame(Stats stats) 
    {
        currentGameTime = 0;
        gameStats = stats;
        gameState = GameState.Playing;
        playerManager.ResetPlayer();
        hudManager.ResetHUD(playerManager.GetMaxHealth());
    }

    public void PauseGame()
    {

    }

    public GameState GetGameState()
    {
        return gameState;
    }

    #endregion

    #region Logic Behaviour

    private void IncreaseGameRythmSpeed()
    {
        if (currentSpeed >= maxSpeed)
            return;

        elapsedTime += Time.deltaTime;

        float newSpeed = minSpeed + (maxSpeed - minSpeed) * (1 - Mathf.Exp(-speedIncreaseRate * elapsedTime));

        if (Mathf.Approximately(newSpeed, currentSpeed))
            return;

        currentSpeed = newSpeed;
        if (currentSpeed > maxSpeed) currentSpeed = maxSpeed;
    }

    private void IncreaseTime()
    {
        currentGameTime += Time.deltaTime;
        hudManager.UpdateTime(currentGameTime);
    }

    #endregion

    #region GameStats

    private void OnEnable()
    {
        ProjectileController.OnProjectileReflected += ProjectileReflected;
        ProjectileController.OnEnemyKilled += EnemyKilled;
    }

    private void OnDisable()
    {
        ProjectileController.OnProjectileReflected -= ProjectileReflected;
        ProjectileController.OnEnemyKilled -= EnemyKilled;
    }

    private void ProjectileReflected()
    {
        gameStats.projectilesReflected++;
    }

    private void EnemyKilled(EnemyType enemyType)
    {
        if (gameStats.enemiesKilled.ContainsKey(enemyType))
            gameStats.enemiesKilled[enemyType]++;
        else
            gameStats.enemiesKilled.Add(enemyType, 1);
    }

    #endregion
}
