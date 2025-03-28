using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private float maxHealth;


    private float health = 0;
    private GamePlayManager gamePlayManager;

    public void SetUp(GamePlayManager gpManager) 
    {
        gamePlayManager = gpManager;    
    }

    private void OnEnable()
    {
        Projectile.OnPlayerHit += ReceiveDamage;
        GamePlayManager.OnGameOver += GameOver;
    }

    private void OnDisable()
    {
        Projectile.OnPlayerHit -= ReceiveDamage;
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

    #endregion

    public void ReceiveDamage(float damage)
    {
        health -= damage;
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
