using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField]
    private Button restartSceneButton;

    [SerializeField]
    private Button startSceneButton;

    [SerializeField]
    private Slider healthBar;

    [SerializeField]
    private TextMeshProUGUI timeText;

    private GamePlayManager gamePlayManager;

    private float time = 0;

    public void SetUp(GamePlayManager gpManager, float playerMaxHP)
    {
        gamePlayManager = gpManager;
        restartSceneButton.gameObject.SetActive(false);
        startSceneButton.gameObject.SetActive(true);

        startSceneButton.onClick.AddListener(() =>
        {
            startSceneButton.gameObject.SetActive(false);
            gamePlayManager.StartGame();
        });

        restartSceneButton.onClick.AddListener(() =>
        {
            restartSceneButton.gameObject.SetActive(false);
            gamePlayManager.RestartGame();
        });

        healthBar.maxValue = playerMaxHP;
        healthBar.value = playerMaxHP;

        time = 0;
        timeText.text = $"{time.ToString("F0")}";
    }


    private void OnEnable()
    {
        GamePlayManager.OnGameOver += GameOver;
    }

    private void OnDisable()
    {
        GamePlayManager.OnGameOver -= GameOver;
    }

    public void UpdateHealth(float health)
    {
        healthBar.value = health;
    }

    public void UpdateTime(float newTime)
    {
        time = newTime;
        timeText.text = $"{time.ToString("F0")}";
    }

    public void UpdateHUD()
    {

    }

    public void StartGame()
    {

    }

    public void GameOver()
    {

    }
}
