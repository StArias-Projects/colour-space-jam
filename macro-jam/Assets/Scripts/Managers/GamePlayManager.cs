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
    public ProjectileManager projectileManager;

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
    private float minGamePlaySpeed;

    [SerializeField]
    [Min(0)]
    private float maxGamePlaySpeed;

    [SerializeField]
    private float speedIncreaseRate;

    #endregion

    #region Private Variables

    private float currentSpeed = 0;
    public float CurrentSpeed { get { return currentSpeed; } }

    private float elapsedTime = 0f;
    private float currentGameTime = 0;
    private GameState gameState = GameState.Opening;
    private Stats gameStats;
    private GameManager gameManager;
    private AudioManager audioManager;
    private Coroutine musicFadeInCoroutine = null;
    private Coroutine musicFadeOutCoroutine = null;

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
        audioManager = AudioManager.GetInstance();

        gameStats.enemiesKilled = new();
        currentSpeed = minGamePlaySpeed;

        playerManager.SetUp(this);
        hudManager.SetUp(this, playerManager.GetMaxHealth());
        enemyManager.SetUp(this, projectileManager, playerManager.transform);
        projectileManager.SetUp(enemyManager, vfxManager);
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
        currentSpeed = minGamePlaySpeed;

        musicFadeInCoroutine = StartCoroutine(AudioManager.MusicFadeIn(gamePlayMusicEmitter, false, 1f));
    }

    public IEnumerator GameOver()
    {
        gameState = GameState.GameOver;
        gameManager.ChangeCursorTexture(gameState);

        gameStats.time = (uint)currentGameTime;
        currentSpeed = minGamePlaySpeed;

        playerManager.StartDeathAnimation();
        OnGameOver?.Invoke();

        yield return new WaitUntil(() => playerManager.IsDeathAnimationFinished());

        pauseMusicEmitter.Stop();
        HandleMusicFadeInFadeOut(gameOverMusicEmitter, false, 3f, gamePlayMusicEmitter, false, 0.5f);

        gameOverManager.GameOver(gameStats);
    }

    public void TryAgain()
    {
        gameManager.InitStats();
        currentGameTime = 0;
        currentSpeed = minGamePlaySpeed;
        gameStats = gameManager.GetGamestats();
        gameState = GameState.Playing;
        gameManager.ChangeCursorTexture(gameState);

        pauseMusicEmitter.Stop();
        HandleMusicFadeInFadeOut(gamePlayMusicEmitter, false, 1f, gameOverMusicEmitter, false, 1f);

        playerManager.ResetPlayer();
        hudManager.ResetHUD(playerManager.GetMaxHealth());
    }

    public void ReturnToMainMenu()
    {
        if(musicFadeInCoroutine != null)
            StopCoroutine(musicFadeInCoroutine);
        if(musicFadeOutCoroutine != null)
            StopCoroutine(musicFadeOutCoroutine);

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

        HandleMusicFadeInFadeOut(pauseMusicEmitter, false, 2f, gamePlayMusicEmitter, true, 1f);

        Time.timeScale = 0;
        OnGamePaused?.Invoke();
    }

    public void ContinueGame()
    {
        gameState = GameState.Playing;
        gameManager.ChangeCursorTexture(gameState);

        HandleMusicFadeInFadeOut(gamePlayMusicEmitter, true, 1f, pauseMusicEmitter, false, 1f);

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
        if (currentSpeed >= maxGamePlaySpeed)
            return;

        elapsedTime += Time.deltaTime;

        float newSpeed = minGamePlaySpeed + (maxGamePlaySpeed - minGamePlaySpeed) * (1 - Mathf.Exp(-speedIncreaseRate * elapsedTime));

        if (Mathf.Approximately(newSpeed, currentSpeed))
            return;

        currentSpeed = newSpeed;
        if (currentSpeed > maxGamePlaySpeed) currentSpeed = maxGamePlaySpeed;
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

    #region Misc

    private void HandleMusicFadeInFadeOut(StudioEventEmitter musicIn, bool isContinued, float fadeInSeconds, 
                                            StudioEventEmitter musicOut, bool isPaused, float fadeOutSeconds) 
    {
        if (musicFadeInCoroutine != null)
            StopCoroutine(musicFadeInCoroutine);
        musicFadeInCoroutine = StartCoroutine(AudioManager.MusicFadeIn(musicIn, isContinued, fadeInSeconds));

        if (musicFadeOutCoroutine != null)
            StopCoroutine(musicFadeOutCoroutine);
        musicFadeOutCoroutine =StartCoroutine(AudioManager.MusicFadeOut(musicOut, isPaused, fadeOutSeconds));
    }

    #endregion
}
