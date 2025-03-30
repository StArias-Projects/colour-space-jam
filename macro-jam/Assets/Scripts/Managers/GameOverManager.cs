using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [SerializeField]
    private GameObject gameOverCanvas;

    [SerializeField]
    private Button tryAgainButton;

    [SerializeField]
    private Button mainMenuButton;

    [SerializeField]
    private TextMeshProUGUI timeText;

    [SerializeField]
    private TextMeshProUGUI projectilesReflectedText;

    [SerializeField]
    private TextMeshProUGUI enemiesKilledText;

    [SerializeField]
    private TextMeshProUGUI favouriteEnemyText;

    private GamePlayManager gamePlayManager;
    private Stats gameStats;

    public void SetUp(GamePlayManager manager)
    {
        gamePlayManager = manager;
        gameStats.enemiesKilled = new();

        tryAgainButton.onClick.AddListener(() => TryAgain());
        mainMenuButton.onClick.AddListener(() => ReturnToMainMenu());

        UpdateCanvas();
    }

    public void GameOver(Stats stats)
    {
        gameStats = stats;
        gameOverCanvas.SetActive(true);
        UpdateCanvas();
    }

    private void UpdateCanvas()
    {
        timeText.text = $"TIME: {gameStats.time}s";
        projectilesReflectedText.text = $"Projectiles Reflected: {gameStats.projectilesReflected}";
        CalculateKilledEnemies();
    }

    private void CalculateKilledEnemies()
    {
        uint kills = 0;
        uint enemyTypeCounter = 0;
        EnemyType favouriteEnemy = EnemyType.None;

        if (gameStats.enemiesKilled != null)
        {
            foreach (var enemy in gameStats.enemiesKilled)
            {
                if (enemy.Value > enemyTypeCounter)
                    favouriteEnemy = enemy.Key;

                kills += enemy.Value;
            }
        }

        enemiesKilledText.text = $"Enemies Killed: {kills}";
        favouriteEnemyText.text = $"Favourite Enemy: {favouriteEnemy.ToString()}";
    }

    private void TryAgain()
    {
        gameOverCanvas.SetActive(false);
        gamePlayManager.TryAgain();
    }

    private void ReturnToMainMenu()
    {
        gameOverCanvas.SetActive(false);
        gamePlayManager.ReturnToMainMenu();
    }
}
