using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField]
    private GameObject hudCanvas;

    [SerializeField]
    private Slider healthBar;

    [SerializeField]
    private TextMeshProUGUI timeText;

    private GamePlayManager gamePlayManager;

    private float time = 0;

    public void SetUp(GamePlayManager gpManager, float playerMaxHP)
    {
        gamePlayManager = gpManager;

        healthBar.maxValue = playerMaxHP;
        healthBar.value = playerMaxHP;

        time = 0;
        timeText.text = $"{time.ToString("F0")}";
    }

    private void OnEnable()
    {
        PlayerManager.OnPlayerTakeDamage += ReduceHealth;
        GamePlayManager.OnGamePaused += OnGamePaused;
        GamePlayManager.OnGameContinued += OnGameContinued;
    }

    private void OnDisable()
    {
        PlayerManager.OnPlayerTakeDamage -= ReduceHealth;
        GamePlayManager.OnGamePaused -= OnGamePaused;
        GamePlayManager.OnGameContinued -= OnGameContinued;
    }

    public void ReduceHealth(float damage)
    {
        healthBar.value -= damage;
        if (healthBar.value < 0)
            healthBar.value = 0;
    }

    public void UpdateTime(float newTime)
    {
        time = newTime;
        timeText.text = $"{time.ToString("F0")}";
    }

    public void ResetHUD(float health)
    {
        healthBar.value = health;
        time = 0;
        timeText.text = $"{time.ToString("F0")}";
    }

    private void OnGamePaused() 
    {
        hudCanvas.SetActive(false);
    }

    private void OnGameContinued() 
    {
        hudCanvas.SetActive(true);
    }
}
