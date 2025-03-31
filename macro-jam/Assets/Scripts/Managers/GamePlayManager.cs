using DG.Tweening;
using FMODUnity;
using System;
using System.Collections;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    #region Editor Variables

    [Header("Managers")]

    [SerializeField]
    private PlayerManager playerManager;

    [SerializeField]
    private GameOverManager gameOverManager;

    [SerializeField]
    private PauseManager pauseManager;

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

    [Header("Audio")]
    [SerializeField]
    private StudioEventEmitter gamePlayMusicEmitter;

    [SerializeField]
    private StudioEventEmitter pauseMusicEmitter;

    [SerializeField]
    private StudioEventEmitter gameOverMusicEmitter;

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

    public float currentSpeed = 0;
    private float elapsedTime = 0f;
    private float currentGameTime = 0;
    private GameState gameState = GameState.Opening;
    private Stats gameStats;
    private GameManager gameManager;

    public static event Action OnGameOver;
    public static event Action OnGamePaused;
    public static event Action OnGameContinued;
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
        pauseManager.SetUp(this);
        gameOverManager.SetUp(this);

        StartGame();
    }

    private void StartGame()
    {
        gameState = GameState.Playing;
        gameManager.ChangeCursorTexture(gameState);
        currentSpeed = minSpeed;

        gamePlayMusicEmitter.Play();
        pauseMusicEmitter.Stop();
    }

    public IEnumerator GameOver()
    {
        gameState = GameState.GameOver;
        gameManager.ChangeCursorTexture(gameState);

        gameStats.time = (uint)currentGameTime;
        currentSpeed = 0;

        playerManager.StartDeathAnimation();
        OnGameOver?.Invoke();
        
        yield return new WaitUntil(() => playerManager.IsDeathAnimationFinished());

        gamePlayMusicEmitter.Stop();
        pauseMusicEmitter.Stop();
        gameOverMusicEmitter.Play();

        gameOverManager.GameOver(gameStats);
    }

    public void TryAgain() 
    {
        gameManager.InitStats();
        currentGameTime = 0;
        gameStats = gameManager.GetGamestats();
        gameState = GameState.Playing;
        gameManager.ChangeCursorTexture(gameState);

        gamePlayMusicEmitter.Stop();
        pauseMusicEmitter.Stop();
        gameOverMusicEmitter.Play();

        playerManager.ResetPlayer();
        hudManager.ResetHUD(playerManager.GetMaxHealth());
    }

    public void ReturnToMainMenu() 
    {
        gamePlayMusicEmitter.Stop();
        pauseMusicEmitter.Stop();
        gameOverMusicEmitter.Stop();

        gameManager.ReturnToMainMenu();
        Time.timeScale = 1;
    }

    public void PauseGame()
    {
        gameState = GameState.Pause;
        gameManager.ChangeCursorTexture(gameState);

        gamePlayMusicEmitter.EventInstance.setPaused(true);
        pauseMusicEmitter.Play();
        Time.timeScale = 0;
        OnGamePaused?.Invoke();
    }

    public void ContinueGame() 
    {
        gameState = GameState.Playing;
        gameManager.ChangeCursorTexture(gameState);

        gamePlayMusicEmitter.EventInstance.setPaused(false);
        pauseMusicEmitter.Stop();
        Time.timeScale = 1;
        OnGameContinued?.Invoke();
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
