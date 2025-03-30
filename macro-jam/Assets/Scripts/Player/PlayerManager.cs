using DG.Tweening;
using System;
using System.Collections;
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
    private bool isDeathFinished = false;
    private Vector3 initialTr;
    public static event Action<float> OnPlayerTakeDamage;

    public void SetUp(GamePlayManager gpManager) 
    {
        health = maxHealth;
        initialTr = transform.position;
        gamePlayManager = gpManager;
        playerController.SetUp(this);
        shieldController.SetUp(this);
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
        return health/maxHealth;
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
            StartCoroutine(gamePlayManager.GameOver());
        }
    }

    public void StartDeathAnimation() 
    {
        // TODO: This is temporal here and it should go at the end of the animation
        transform.DOScale(Vector3.zero, 1f).OnComplete(FinishDeathAnimation);
    }

    public void FinishDeathAnimation() 
    {
        isDeathFinished = true;
    }

    public void ResetPlayer()
    {
        isDeathFinished = false;
        health = maxHealth;
        transform.position = initialTr;
    }


    public bool IsDeathAnimationFinished() {
        return isDeathFinished; 
    }
}
