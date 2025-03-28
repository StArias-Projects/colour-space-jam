using System.Collections.Generic;
using System;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Serializable]
    public class EnemyPool
    {
        /// <summary>
        /// Reference to the prefab of the enemy
        /// </summary>
        public EnemyController enemyController;
        public int maxSizePool;

        [HideInInspector]
        public List<EnemyController> inactiveEnemies = new List<EnemyController>();
    }

    #region Editor Variables

    [SerializeField]
    private List<EnemyPool> enemyPoolList;


    [SerializeField]
    private Transform enemyContainer;

    [SerializeField]
    private List<Transform> spawnPoints;

    [SerializeField]
    private List<Collider> directionPoints;

    [SerializeField]
    private float generationRate;

    #endregion

    private GamePlayManager gamePlayManager;
    private bool isGenerating = false;
    private float currentTime = 0;

    public static event Action OnGameOver;

    public void SetUp(GamePlayManager gpManager)
    {
        gamePlayManager = gpManager;
    }

    public void StartGame()
    {
        isGenerating = true;
        GenerateEnemies();
    }

    public void GenerateEnemies()
    {
        int poolGroup = 0;

        foreach (EnemyPool pool in enemyPoolList)
        {
            for (int i = 0; i < pool.maxSizePool; i++)
            {
                EnemyController enemy = Instantiate(pool.enemyController, enemyContainer.position, Quaternion.identity, enemyContainer);
                enemy.gameObject.SetActive(false);
                pool.inactiveEnemies.Add(enemy);
                enemy.SetUpEnemy(this, directionPoints, poolGroup);
            }

            poolGroup++;
        }
    }

    private void Update()
    {
        if (!isGenerating)
            return;

        currentTime += Time.deltaTime;

        if (currentTime >= generationRate)
        {
            SpawnEnemy();
            currentTime = 0;
        }
    }

    private void SpawnEnemy()
    {
        int rndEnemy = UnityEngine.Random.Range(0, enemyPoolList.Count);
        int rndSpawnPoint = UnityEngine.Random.Range(0, spawnPoints.Count);

        if (enemyPoolList.Count == 0
            || spawnPoints.Count == 0
            || enemyPoolList[rndEnemy].inactiveEnemies.Count == 0)
            return;

        EnemyController enemy = enemyPoolList[rndEnemy].inactiveEnemies[0];

        Vector3 newPos = spawnPoints[rndSpawnPoint].position;

        enemy.StartEnemy(newPos);
        enemyPoolList[rndEnemy].inactiveEnemies.Remove(enemy);
    }

    public void ResetEnemy(EnemyController enemy)
    {
        enemy.transform.position = enemyContainer.position;
        enemy.gameObject.SetActive(false);
        int group = enemy.GetGroupIndex();
        enemyPoolList[group].inactiveEnemies.Add(enemy);
    }

    public void GameOver()
    {
        isGenerating = false;
        OnGameOver?.Invoke();
    }

    public GamePlayManager.GameState GetGameState() 
    {
        return gamePlayManager.GetGameState();
    }
}
