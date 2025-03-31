using DG.Tweening;
using System;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private float maxHealth;

    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private ShieldController shieldController;
    
    [SerializeField]
    private PlayerSFXController playerSFXController;

    [SerializeField]
    [Range(0, 1)]
    private float heartBeatHealthThreshHold;

    private float health = 0;
    private GamePlayManager gamePlayManager;
    private bool isDeathFinished = false;
    private Vector3 initialPos;
    private Quaternion initialRotation;
    private Vector3 initialScale;
    public static event Action<float, bool> OnPlayerTakeDamage;
    public static event Action<float, bool> OnPlayerHealed;
    public void SetUp(GamePlayManager gpManager)
    {
        health = maxHealth;
        initialPos = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;

        gamePlayManager = gpManager;

        playerController.SetUp(this, playerSFXController);
        shieldController.SetUp(this, playerSFXController);

        playerSFXController.PlaySpawnPlayer();
    }

    private void OnEnable()
    {
        ProjectileController.OnPlayerHit += ReceiveDamage;
    }

    private void OnDisable()
    {
        ProjectileController.OnPlayerHit -= ReceiveDamage;
    }

    #region Getters

    public float GetHealth()
    {
        return health;
    }

    public float GetPercentHealth()
    {
        return health / maxHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public GameState GetGameState()
    {
        return gamePlayManager.GetGameState();
    }

    #endregion

    public void OnPause(CallbackContext context)
    {
        if (gamePlayManager.GetGameState() != GameState.Playing)
            return;

        gamePlayManager.PauseGame();
    }

    public void ReceiveDamage(float damage)
    {
        health -= damage;
        bool isLowHealth = health <= maxHealth * heartBeatHealthThreshHold;

        if (isLowHealth)
            playerSFXController.PlayHeartBeat();

        if (damage > 0)
        {
            OnPlayerTakeDamage?.Invoke(damage, isLowHealth);
        }

        if (health <= 0)
        {
            health = 0;
            StartCoroutine(gamePlayManager.GameOver());
        }
    }

    public void GainHealth(float amount)
    {
        health += amount;
        health = Mathf.Min(health, maxHealth);
        bool isNotLowHealth = health > maxHealth * heartBeatHealthThreshHold;

        if (isNotLowHealth)
            playerSFXController.StopHeartBeat();

        if (amount > 0)
        {
            OnPlayerHealed?.Invoke(amount, isNotLowHealth);
        }
    }

    public void StartDeathAnimation()
    {
        playerSFXController.StopHeartBeat();
        playerSFXController.PlayPlayerDeathSFX();
        transform.DOScale(Vector3.zero, 1f).OnComplete(FinishDeathAnimation);
    }

    public void FinishDeathAnimation()
    {
        isDeathFinished = true;
    }

    public void ResetPlayer()
    {
        playerSFXController.PlaySpawnPlayer();
        isDeathFinished = false;
        health = maxHealth;
        transform.SetPositionAndRotation(initialPos, initialRotation);
        transform.localScale = initialScale;
        shieldController.ResetShields();
    }

    public bool IsDeathAnimationFinished()
    {
        return isDeathFinished;
    }
}
