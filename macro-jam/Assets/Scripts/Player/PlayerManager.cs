using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private float maxHealth;

    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private ShieldController shieldController;

    private float health = 0;
    private GamePlayManager gamePlayManager;


    public static event Action<float> OnPlayerTakeDamage;

    public void SetUp(GamePlayManager gpManager) 
    {
        health = maxHealth;
        gamePlayManager = gpManager;
        playerController.SetUp(this);
        shieldController.SetUp(this);
    }

    private void OnEnable()
    {
        ProjectileController.OnPlayerHit += ReceiveDamage;
        GamePlayManager.OnGameOver += GameOver;
    }

    private void OnDisable()
    {
        ProjectileController.OnPlayerHit -= ReceiveDamage;
        GamePlayManager.OnGameOver -= GameOver;
    }

    #region Getters

    public float GetHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public GamePlayManager.GameState GetGameState()
    {
        return gamePlayManager.GetGameState();
    }

    #endregion

    public void ReceiveDamage(float damage)
    {
        health -= damage;
        if(damage > 0)
        {
            OnPlayerTakeDamage?.Invoke(damage);
        }

        if (health <= 0)
        {
            health = 0;
            gamePlayManager.GameOver();
        }

        gamePlayManager.UpdateHealth(health);
    }

    public void GameOver() 
    {
        
    }
}
