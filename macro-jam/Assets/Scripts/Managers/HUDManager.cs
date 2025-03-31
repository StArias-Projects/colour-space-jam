using System.Collections;
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

    [Header("Health Pulse Effect")]
    [SerializeField]
    private Image healthPulseImage;

    [SerializeField]
    private float minAlpha;

    [SerializeField]
    private float maxAlpha;

    [SerializeField]
    private float alphaSpeed;

    private Color originalHealthPulseColor;

    private GamePlayManager gamePlayManager;

    private float time = 0;
    private bool isLowHealth = false;

    public void SetUp(GamePlayManager gpManager, float playerMaxHP)
    {
        gamePlayManager = gpManager;

        healthBar.maxValue = playerMaxHP;
        healthBar.value = playerMaxHP;

        time = 0;
        timeText.text = $"{time.ToString("F0")}";

        originalHealthPulseColor = healthPulseImage.color;
    }

    private void OnEnable()
    {
        PlayerManager.OnPlayerTakeDamage += ReduceHealth;
        PlayerManager.OnPlayerHealed += GainHealth;
        GamePlayManager.OnGamePaused += OnGamePaused;
        GamePlayManager.OnGameContinued += OnGameContinued;
    }

    private void OnDisable()
    {
        PlayerManager.OnPlayerTakeDamage -= ReduceHealth;
        PlayerManager.OnPlayerHealed -= GainHealth;
        GamePlayManager.OnGamePaused -= OnGamePaused;
        GamePlayManager.OnGameContinued -= OnGameContinued;
    }

    private IEnumerator LowHealthEffect()
    {
        while (isLowHealth) // Runs continuously until stopped
        {
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, Mathf.PingPong(Time.time * alphaSpeed, 1));
            Color color = healthPulseImage.color;
            color.a = alpha;

            healthPulseImage.color = color;
            yield return null; // Waits until next frame
        }

        healthPulseImage.color = originalHealthPulseColor;
    }

    public void ReduceHealth(float damage, bool enablePulsHealthVFX)
    {
        healthBar.value -= damage;
        if (healthBar.value < 0)
            healthBar.value = 0;

        if (!isLowHealth && enablePulsHealthVFX) 
        {
            isLowHealth = true;
            StartCoroutine(LowHealthEffect());
        }
    }

    public void GainHealth(float amount, bool disablePulseHealthVFX)
    {
        healthBar.value += amount;
        if (healthBar.value > healthBar.maxValue)
            healthBar.value = healthBar.maxValue;

        if (isLowHealth && disablePulseHealthVFX)
            isLowHealth = false;
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
        isLowHealth = false;
        healthPulseImage.color = originalHealthPulseColor;
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
